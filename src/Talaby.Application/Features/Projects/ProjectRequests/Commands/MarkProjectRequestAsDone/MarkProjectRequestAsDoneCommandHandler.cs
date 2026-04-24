using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users;
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
    IUserContext userContext,
    IUnitOfWork unitOfWork,
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

        // --- Create the commission payment (snapshotting amount and percentage at this point in time) ---
        var commissionPayment = ProjectCommissionPayment.Create(
            projectRequestId: request.ProjectRequestId,
            projectProposalId: acceptedProposal.Id,
            proposalAmountSnapshot: acceptedProposal.ProposedAmount,
            commissionPercentage: tapCheckoutOptions.Value.CommissionPercentage,
            currency: DefaultCurrency,
            createdAtUtc: DateTime.UtcNow);

        await commissionPaymentRepository.AddAsync(commissionPayment, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Project request {ProjectRequestId} marked as AwaitingCommissionPayment. " +
            "Commission payment {CommissionPaymentId} created for {CommissionAmount} {Currency}.",
            request.ProjectRequestId,
            commissionPayment.Id,
            commissionPayment.CommissionAmount,
            commissionPayment.Currency);
    }
}
