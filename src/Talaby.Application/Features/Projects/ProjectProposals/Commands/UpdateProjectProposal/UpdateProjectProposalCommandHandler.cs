using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposal;

public class UpdateProjectProposalCommandHandler(ILogger<UpdateProjectProposalCommandHandler> logger,
    IProjectProposalRepository projectProposalRepository, IMapper mapper) : IRequestHandler<UpdateProjectProposalCommand>
{
    public async Task Handle(UpdateProjectProposalCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProjectProposal with id: {ProjectProposalId} with {@UpdatedProjectProposal}", request.Id, request);
        var projectProposal = await projectProposalRepository.GetByIdAsync(request.Id);
        if (projectProposal is null)
            throw new NotFoundException(nameof(ProjectProposal), request.Id.ToString());

        var existingProjectProposal = await projectProposalRepository.AnyAsync( p => p.Id == request.Id  , cancellationToken);

        if (!existingProjectProposal)
        {
            throw new NotFoundException(nameof(ProjectProposal), request.Id.ToString());
        }

        mapper.Map(request, projectProposal);

        await projectProposalRepository.SaveChanges();
    }
}
