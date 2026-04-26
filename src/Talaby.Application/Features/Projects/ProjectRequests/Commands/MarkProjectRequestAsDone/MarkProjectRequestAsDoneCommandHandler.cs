using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users;
using Talaby.Domain.Entities;
using Talaby.Domain.Entities.Payments;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Payments;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.MarkProjectRequestAsDone;

public class MarkProjectRequestAsDoneCommandHandler(
    IProjectRequestRepository projectRequestRepository,
    IProjectProposalRepository projectProposalRepository,
    IProjectCommissionPaymentRepository commissionPaymentRepository,
    UserManager<AppUser> userManager,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IMailService mailService,
    IOptions<TapCheckoutOptions> tapCheckoutOptions,
    ILogger<MarkProjectRequestAsDoneCommandHandler> logger)
    : IRequestHandler<MarkProjectRequestAsDoneCommand>
{
    private const string DefaultCurrency = "SAR";

    public async Task Handle(MarkProjectRequestAsDoneCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Store marking project request {ProjectRequestId} as done.", request.ProjectRequestId);

        var currentUser = userContext.GetCurrentUser()
            ?? throw new UnAuthorizedAccessException("User not authenticated.");

        // --- Load the project request ---
        var projectRequest = await projectRequestRepository.GetByIdAsync(request.ProjectRequestId);
        if (projectRequest is null)
            throw new NotFoundException(nameof(ProjectRequest), request.ProjectRequestId.ToString());

        // --- Business rule: must be InProgress ---
        if (projectRequest.Status != ProjectRequestStatus.InProgress)
            throw new BusinessRuleException(
                "Project must be InProgress before it can be marked as done.", 409);

        // --- Find the accepted proposal for this request ---
        var acceptedProposals = await projectProposalRepository.GetAllAsync(
            pp => pp.ProjectRequestId == request.ProjectRequestId
                  && pp.Status == ProjectProposalStatus.Accepted,
            cancellationToken);

        var acceptedProposal = acceptedProposals?.FirstOrDefault();
        if (acceptedProposal is null)
            throw new BusinessRuleException(
                "No accepted proposal found for this project request.", 409);

        // --- Authorization: only the store that owns the accepted proposal ---
        if (acceptedProposal.CreatorId != currentUser.Id)
            throw new BusinessRuleException(
                "Only the store that submitted the accepted proposal can mark the project as done.", 403);

        // --- Guard against duplicate commission payment records ---
        var paymentAlreadyExists = await commissionPaymentRepository
            .ExistsForProjectAsync(request.ProjectRequestId, cancellationToken);

        if (paymentAlreadyExists)
            throw new BusinessRuleException(
                "A commission payment record already exists for this project request.", 409);

        // --- State transition ---
        projectRequest.MarkAwaitingCommissionPayment();
        acceptedProposal.Complete();

        // --- Create the commission payment (snapshotting amount and percentage at this point in time) ---
        var opts = tapCheckoutOptions.Value;
        var commissionPayment = ProjectCommissionPayment.Create(
            projectRequestId: request.ProjectRequestId,
            projectProposalId: acceptedProposal.Id,
            proposalAmountSnapshot: acceptedProposal.ProposedAmount,
            commissionPercentage: opts.CommissionPercentage,
            currency: DefaultCurrency,
            createdAtUtc: DateTime.UtcNow);

        await commissionPaymentRepository.AddAsync(commissionPayment, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Project request {ProjectRequestId} marked as AwaitingCommissionPayment. " +
            "CommissionPaymentId={CommissionPaymentId}, CommissionAmount={CommissionAmount} {Currency}.",
            request.ProjectRequestId,
            commissionPayment.Id,
            commissionPayment.CommissionAmount,
            commissionPayment.Currency);

        // --- Notify the client via email (best-effort — failure must not block the operation) ---
        await SendCommissionNotificationAsync(
            projectRequest, commissionPayment, opts.FrontendBaseUrl, cancellationToken);
    }

    private async Task SendCommissionNotificationAsync(
        ProjectRequest projectRequest,
        ProjectCommissionPayment commissionPayment,
        string frontendBaseUrl,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = await userManager.FindByIdAsync(projectRequest.CreatorId.ToString());
            if (client?.Email is null)
            {
                logger.LogWarning(
                    "Commission notification skipped: client email not found. ProjectRequestId={ProjectRequestId}, ClientId={ClientId}",
                    projectRequest.Id, projectRequest.CreatorId);
                return;
            }

            var appUrl = $"{frontendBaseUrl.TrimEnd('/')}/projects/{projectRequest.Id}/payment";
            var firstName = string.IsNullOrWhiteSpace(client.FirstName) ? "there" : client.FirstName;
            var amountFormatted = $"{commissionPayment.CommissionAmount:0.00} {commissionPayment.Currency}";

            var body = $"""
                <p>Hi {firstName},</p>
                <p>Great news — the store has marked your project <strong>{projectRequest.Title}</strong> as completed!</p>
                <p>To finalise the project, please pay the platform commission of <strong>{amountFormatted}</strong>.</p>
                <p><a href="{appUrl}" style="background:#1a73e8;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;">Pay Commission Now</a></p>
                <p>If you have any questions, please contact our support team.</p>
                <p>Best regards,<br/>Talaby Team</p>
                """;

            await mailService.SendEmailAsync(
                to: client.Email,
                subject: $"Action Required: Pay Commission for \"{projectRequest.Title}\"",
                body: body,
                attachmentPaths: null);

            logger.LogInformation(
                "Commission notification sent. ProjectRequestId={ProjectRequestId}, ClientId={ClientId}",
                projectRequest.Id, projectRequest.CreatorId);
        }
        catch (Exception ex)
        {
            // Email failure must never roll back the already-committed domain state.
            logger.LogWarning(
                ex,
                "Commission notification failed to send. ProjectRequestId={ProjectRequestId}, ClientId={ClientId}",
                projectRequest.Id, projectRequest.CreatorId);
        }
    }
}
