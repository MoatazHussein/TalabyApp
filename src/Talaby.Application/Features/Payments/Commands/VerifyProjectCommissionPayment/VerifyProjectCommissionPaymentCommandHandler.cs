using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Application.Features.Payments.Services;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities.Payments;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Payments;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Payments.Commands.VerifyProjectCommissionPayment;

public sealed class VerifyProjectCommissionPaymentCommandHandler(
    IProjectRequestRepository projectRequestRepository,
    IProjectCommissionPaymentRepository commissionPaymentRepository,
    ITapPaymentService tapPaymentService,
    ICommissionPaymentReconciler reconciler,
    IUserContext userContext,
    ITimeZoneConverter timeZoneConverter,
    IUnitOfWork unitOfWork,
    ILogger<VerifyProjectCommissionPaymentCommandHandler> logger)
    : IRequestHandler<VerifyProjectCommissionPaymentCommand, VerifyProjectCommissionPaymentResponse>
{
    public async Task<VerifyProjectCommissionPaymentResponse> Handle(
        VerifyProjectCommissionPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()
            ?? throw new UnAuthorizedAccessException("User is not authenticated.");

        var projectRequest = await projectRequestRepository.GetByIdAsync(request.ProjectRequestId)
            ?? throw new NotFoundException("ProjectRequest", request.ProjectRequestId.ToString());

        var isOwner = projectRequest.CreatorId == currentUser.Id;
        var isAdmin = currentUser.IsInRole(UserRoles.Admin);

        if (!isOwner && !isAdmin)
            throw new UnAuthorizedAccessException("Access denied. You are not the owner of this project request.");

        var commissionPayment = await commissionPaymentRepository
            .GetWithAttemptsByProjectRequestIdAsync(request.ProjectRequestId, cancellationToken);

        if (commissionPayment is not null && !commissionPayment.IsFinalState())
        {
            var latestAttempt = commissionPayment.PaymentAttempts
                .Where(a => !string.IsNullOrWhiteSpace(a.ProviderChargeId))
                .MaxBy(a => a.CreatedAtUtc);

            if (latestAttempt is not null)
                await SyncFromTapAsync(commissionPayment, latestAttempt, cancellationToken);
        }

        logger.LogDebug(
            "Verified commission payment. ProjectRequestId={ProjectRequestId}, UserId={UserId}, ProjectStatus={ProjectStatus}, PaymentStatus={PaymentStatus}",
            request.ProjectRequestId, currentUser.Id, projectRequest.Status, commissionPayment?.Status);

        return BuildResponse(projectRequest.Id, projectRequest.Status, commissionPayment);
    }

    private async Task SyncFromTapAsync(
        ProjectCommissionPayment commissionPayment,
        ProjectCommissionPaymentAttempt latestAttempt,
        CancellationToken cancellationToken)
    {
        TapRetrieveChargeResponse tapCharge;
        try
        {
            tapCharge = await tapPaymentService.RetrieveChargeAsync(
                latestAttempt.ProviderChargeId!, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "VerifyProjectCommissionPayment: Tap retrieve-charge failed. CommissionPaymentId={CommissionPaymentId}, ChargeId={ChargeId}. Local state unchanged.",
                commissionPayment.Id, latestAttempt.ProviderChargeId);
            return;
        }

        var outcome = TapChargeStatusMapper.Map(tapCharge.ProviderStatus);

        await reconciler.ApplyChargeOutcomeAsync(
            commissionPayment, latestAttempt, outcome, tapCharge.FailureMessage, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private VerifyProjectCommissionPaymentResponse BuildResponse(
        Guid projectRequestId,
        ProjectRequestStatus projectStatus,
        ProjectCommissionPayment? commissionPayment)
    {
        var response = new VerifyProjectCommissionPaymentResponse(
            ProjectRequestId: projectRequestId,
            ProjectStatus: projectStatus,
            PaymentStatus: commissionPayment?.Status,
            IsPaid: commissionPayment?.Status == ProjectCommissionPaymentStatus.Paid,
            PaidAt: commissionPayment?.PaidAtUtc);

        return timeZoneConverter.ConvertUtcToLocal(response);
    }
}
