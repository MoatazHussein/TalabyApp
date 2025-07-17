using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposalStatus;

public class UpdateProjectProposalStatusCommandHandler(ILogger<UpdateProjectProposalStatusCommandHandler> logger, IUserContext userContext,
    IProjectRequestRepository projectRequestRepository, IProjectProposalRepository projectProposalRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProjectProposalStatusCommand>
{
    public async Task Handle(UpdateProjectProposalStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProjectProposal with id: {ProjectProposalId} with {@UpdatedProjectProposal}", request.Id, request);
        var projectProposal = await projectProposalRepository.GetByIdAsync(request.Id);
        if (projectProposal is null)
            throw new NotFoundException(nameof(ProjectProposal), request.Id.ToString());

        var currentUser = userContext.GetCurrentUser()
                     ?? throw new UnAuthorizedAccessException("User not authenticated.");

        var projectRequest = await projectRequestRepository.GetByIdAsync(projectProposal.ProjectRequestId);
        if (projectRequest is null)
            throw new NotFoundException(nameof(ProjectRequest), projectProposal.ProjectRequestId.ToString());


        //if (projectRequest.Status == ProjectRequestStatus.Closed)
        //    throw new BusinessRuleException("can't change status for proposal of closed project", 409);

        var allowedStoreActions = new[] { ProjectProposalStatus.Cancelled };
        var allowedClientActions = new[] { ProjectProposalStatus.Accepted, ProjectProposalStatus.Rejected , ProjectProposalStatus.Cancelled };


        if (projectProposal.CreatorId == currentUser.Id && !allowedStoreActions.Contains(request.NewStatus))
            throw new BusinessRuleException($"You are only allowed to update with {string.Join(", ", allowedStoreActions)}", 403);


        if (projectRequest.CreatorId == currentUser.Id && !allowedClientActions.Contains(request.NewStatus))
            throw new BusinessRuleException($"You are only allowed to update with {string.Join(", ", allowedClientActions)}", 403);

        if (projectProposal.CreatorId != currentUser.Id && projectRequest.CreatorId != currentUser.Id)
            throw new BusinessRuleException("You are not allowed to update this Proposal.", 403);

        if (request.NewStatus == ProjectProposalStatus.Accepted && projectProposal.Status == ProjectProposalStatus.Accepted)
            throw new BusinessRuleException("This proposal is already accepted.", 400);

        var acceptedProjectProposal = await projectProposalRepository.GetAllAsync(
            pp => pp.ProjectRequestId == projectRequest.Id && pp.Status == ProjectProposalStatus.Accepted);

        if (acceptedProjectProposal != null && acceptedProjectProposal.Any(pp=> pp.Id != projectProposal.Id) && request.NewStatus == ProjectProposalStatus.Accepted)
            throw new BusinessRuleException("Another proposal has already been accepted for this project request.", 409);

        projectProposal.Status = request.NewStatus;

        // If the proposal is accepted, close the project request 
        if (request.NewStatus == ProjectProposalStatus.Accepted)
        {
            projectRequest.Status = ProjectRequestStatus.Closed;

            var otherProposals = await projectProposalRepository.GetAllAsync(
                pp => pp.ProjectRequestId == projectRequest.Id
                           && pp.Id != request.Id
                           && pp.Status == ProjectProposalStatus.Pending

                , cancellationToken);

            foreach (var other in otherProposals)
            {
                other.Status = ProjectProposalStatus.Rejected;
            }
        }

        var projectInitiationStatuses = new[] { ProjectProposalStatus.Rejected, ProjectProposalStatus.Cancelled };

        if (projectInitiationStatuses.Contains(request.NewStatus))
        {
            projectRequest.Status = ProjectRequestStatus.Open;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
