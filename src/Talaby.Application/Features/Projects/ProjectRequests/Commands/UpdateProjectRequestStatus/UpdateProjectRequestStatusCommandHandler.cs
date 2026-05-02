using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequestStatus;

public class UpdateProjectRequestStatusCommandHandler(ILogger<UpdateProjectRequestStatusCommandHandler> logger, IUserContext userContext,
    IProjectRequestRepository projectRequestRepository,
    IProjectProposalRepository projectProposalRepository,
    IUserPolicyViolationService userPolicyViolationService,
    IUserActionGuard userActionGuard) : IRequestHandler<UpdateProjectRequestStatusCommand>
{
    public async Task Handle(UpdateProjectRequestStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProjectRequest with id: {ProjectRequestId} with {@UpdatedProjectRequest}", request.Id, request);

        var projectRequest = await projectRequestRepository.GetByIdAsync(request.Id);

        if (projectRequest is null)
            throw new NotFoundException(nameof(ProjectRequest), request.Id.ToString());


        var currentUser = userContext.GetCurrentUser()
          ?? throw new UnAuthorizedAccessException("User not authenticated.");

        if (projectRequest.CreatorId != currentUser.Id)
            throw new BusinessRuleException("You are not allowed to Update this project.", 403);

        if (request.NewStatus != ProjectRequestStatus.Cancelled)
            throw new BusinessRuleException(
                "Direct status updates are only permitted for cancellation.", 422);

        var hasAcceptedProposal = await projectProposalRepository.AnyAsync(
            proposal => proposal.ProjectRequestId == projectRequest.Id
                        && proposal.Status == ProjectProposalStatus.Accepted,
            cancellationToken);

        if (hasAcceptedProposal)
        {
            await userActionGuard.EnsureCanCancelAcceptedWorkAsync(
                currentUser.Id,
                cancellationToken);
        }

        await userPolicyViolationService.RecordAcceptedProjectCancellationAsync(
            projectRequest.Id,
            cancellationToken);

        projectRequest.MarkCancelled(request.CancellationReason, currentUser.Id);

        await projectRequestRepository.SaveChanges();
    }
}
