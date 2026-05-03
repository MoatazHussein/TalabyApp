using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.DeleteProjectProposal
{
    internal class DeleteProjectProposalCommandHandler(ILogger<DeleteProjectProposalCommandHandler> logger,IUserContext userContext,
    IProjectProposalRepository ProjectProposalRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProjectProposalCommand>
    {
        public async Task Handle(DeleteProjectProposalCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting ProjectProposal with id: {ProjectProposalId}", request.Id);
            var projectProposal = await ProjectProposalRepository.GetByIdAsync(request.Id);
            if (projectProposal is null)
                throw new NotFoundException(nameof(ProjectProposal), request.Id.ToString());

            var currentUser = userContext.GetCurrentUser()
                    ?? throw new UnAuthorizedAccessException("User not authenticated.");

            if (projectProposal.CreatorId != currentUser.Id)
                throw new BusinessRuleException("You are not allowed to delete this Proposal.", 403);


            if (projectProposal.Status != ProjectProposalStatus.Pending)
                throw new BusinessRuleException($"Can't delete Proposal with {projectProposal.Status.ToString()} status", 409);

            await ProjectProposalRepository.Delete(projectProposal);
            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
