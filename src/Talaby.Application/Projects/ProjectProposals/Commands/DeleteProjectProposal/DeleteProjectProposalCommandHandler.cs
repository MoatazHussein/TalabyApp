using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.ProjectProposals.Commands.DeleteProjectProposal
{
    internal class DeleteProjectProposalCommandHandler(ILogger<DeleteProjectProposalCommandHandler> logger,
    IProjectProposalRepository ProjectProposalRepository) : IRequestHandler<DeleteProjectProposalCommand>
    {
        public async Task Handle(DeleteProjectProposalCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting ProjectProposal with id: {ProjectProposalId}", request.Id);
            var projectProposal = await ProjectProposalRepository.GetByIdAsync(request.Id);
            if (projectProposal is null)
                throw new NotFoundException(nameof(ProjectProposal), request.Id.ToString());

            await ProjectProposalRepository.Delete(projectProposal);

        }
    }
}
