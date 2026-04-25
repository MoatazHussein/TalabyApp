using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Application.Features.Users;
using Talaby.Domain.Entities;
using Talaby.Domain.Entities.Payments;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Payments;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Payments.Commands.CreateProjectCommissionCheckout;

public class CreateProjectCommissionCheckoutCommandHandler(
    IProjectRequestRepository projectRequestRepository,
    IProjectCommissionPaymentRepository commissionPaymentRepository,
    UserManager<AppUser> userManager,
    IUserContext userContext,
    ITapPaymentService tapPaymentService,
    IOptions<TapCheckoutOptions> tapCheckoutOptions,
    IUnitOfWork unitOfWork,
    ILogger<CreateProjectCommissionCheckoutCommandHandler> logger)
    : IRequestHandler<CreateProjectCommissionCheckoutCommand, CreateProjectCommissionCheckoutResponse>
{
    public async Task<CreateProjectCommissionCheckoutResponse> Handle(
        CreateProjectCommissionCheckoutCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()
            ?? throw new UnAuthorizedAccessException("User not authenticated.");

        // 1. Load the project request
        var projectRequest = await projectRequestRepository.GetByIdAsync(request.ProjectRequestId);
        if (projectRequest is null)
            throw new NotFoundException(nameof(ProjectRequest), request.ProjectRequestId.ToString());

        // 2. Only the client who owns this project can initiate commission payment
        if (projectRequest.CreatorId != currentUser.Id)
            throw new BusinessRuleException(
                "You are not authorized to initiate commission payment for this project.", 403);

        // 3. Project must be awaiting commission payment
        if (projectRequest.Status != ProjectRequestStatus.AwaitingCommissionPayment)
            throw new BusinessRuleException(
                $"Commission payment can only be initiated when the project is AwaitingCommissionPayment. " +
                $"Current status: {projectRequest.Status}.", 409);

        // 4. Load the commission payment record (source of truth for amount/currency)
        var commissionPayment = await commissionPaymentRepository
            .GetByProjectRequestIdAsync(request.ProjectRequestId, cancellationToken);

        if (commissionPayment is null)
            throw new NotFoundException(nameof(ProjectCommissionPayment), request.ProjectRequestId.ToString());

        // 5. Guard: already paid — no re-initiation needed
        if (commissionPayment.Status == ProjectCommissionPaymentStatus.Paid)
            throw new BusinessRuleException(
                "The commission for this project has already been paid.", 409);

        // 6. Load the client's full profile for Tap customer fields.
        var client = await userManager.FindByIdAsync(projectRequest.CreatorId.ToString());

        // 7. Build per-attempt references.
        var correlationId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var providerTransactionRef = $"TXN-{correlationId:N}";
        var providerPaymentRef = $"ORD-{commissionPayment.Id:N}";
        var idempotencyKey = $"ic-{correlationId:N}";

        // 8. Create the attempt (status: Initiated) and stage it for persistence
        var attempt = ProjectCommissionPaymentAttempt.CreateInitiated(
            projectCommissionPaymentId: commissionPayment.Id,
            providerName: PaymentProvider.Tap,
            providerTransactionReference: providerTransactionRef,
            providerPaymentReference: providerPaymentRef,
            idempotencyKey: idempotencyKey,
            requestedAmount: commissionPayment.CommissionAmount,
            requestedCurrency: commissionPayment.Currency,
            createdAtUtc: now);

        await commissionPaymentRepository.AddAttemptAsync(attempt, cancellationToken);

        logger.LogInformation(
            "Commission checkout initiated. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}, AttemptId={AttemptId}, UserId={UserId}",
            request.ProjectRequestId, commissionPayment.Id, attempt.Id, currentUser.Id);

        // 9. Build the Tap create-charge request using only snapshotted values
        var tapOpts = tapCheckoutOptions.Value;
        var (phoneCountry, phoneNumber) = ParsePhone(client?.Mobile);

        var tapRequest = new TapCreateChargeRequest(
            Amount: commissionPayment.CommissionAmount,
            Currency: commissionPayment.Currency,
            Description: $"Platform commission for project request {request.ProjectRequestId:N}",
            ProviderTransactionReference: providerTransactionRef,
            ProviderPaymentReference: providerPaymentRef,
            IdempotencyKey: idempotencyKey,
            CustomerName: BuildCustomerName(client, currentUser.Email),
            CustomerEmail: client?.Email ?? currentUser.Email,
            CustomerPhoneCountryCode: phoneCountry,
            CustomerPhoneNumber: phoneNumber,
            SourceId: tapOpts.SourceId,
            RedirectUrl: $"{tapOpts.FrontendBaseUrl.TrimEnd('/')}/payment/result",
            PostUrl: $"{tapOpts.ApiPublicBaseUrl.TrimEnd('/')}/api/payments/tap/webhook"
            );

        // 10. Call Tap — on failure: persist the failed attempt and surface a clean error
        TapCreateChargeResponse tapResponse;
        try
        {
            tapResponse = await tapPaymentService.CreateChargeAsync(tapRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Tap create-charge failed. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}, AttemptId={AttemptId}, CorrelationId={CorrelationId}",
                request.ProjectRequestId, commissionPayment.Id, attempt.Id, correlationId);

            var failureReason = ex.Message.Length > 500 ? ex.Message[..500] : ex.Message;
            attempt.SetFailed(failureReason, DateTime.UtcNow);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            throw new BusinessRuleException(
                "Payment gateway is temporarily unavailable. Please try again later.", 502);
        }

        // 11. Success — record the provider charge id + URL, move payment to Initiated
        attempt.SetCheckoutUrlGenerated(
            providerChargeId: tapResponse.ProviderChargeId,
            checkoutUrl: tapResponse.CheckoutUrl,
            updatedAtUtc: DateTime.UtcNow);

        commissionPayment.MarkInitiated();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Tap checkout created. CommissionPaymentId={PaymentId}, AttemptId={AttemptId}, ChargeId={ChargeId}",
            commissionPayment.Id, attempt.Id, tapResponse.ProviderChargeId);

        return new CreateProjectCommissionCheckoutResponse(
            ProjectCommissionPaymentId: commissionPayment.Id,
            ProjectCommissionPaymentAttemptId: attempt.Id,
            ProviderChargeId: tapResponse.ProviderChargeId,
            CheckoutUrl: tapResponse.CheckoutUrl,
            ProviderStatus: tapResponse.ProviderStatus);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    private static (string CountryCode, string Number) ParsePhone(string? mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            return ("966", "00000000");

        var digits = new string(mobile.Where(char.IsDigit).ToArray());
        if (digits.Length == 0)
            return ("966", "00000000");

        if (digits.Length > 8 && digits.StartsWith("966"))
            return ("966", digits[3..]);

        var number = digits.TrimStart('0');
        return string.IsNullOrEmpty(number)
            ? ("966", "00000000")
            : ("966", number);
    }

    private static string BuildCustomerName(AppUser? user, string fallbackEmail)
    {
        if (user is null) return fallbackEmail;

        var name = string.Join(" ", new[] { user.FirstName, user.LastName }
            .Where(p => !string.IsNullOrWhiteSpace(p))).Trim();

        return string.IsNullOrEmpty(name) ? user.Email ?? fallbackEmail : name;
    }
}
