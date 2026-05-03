using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposal;

public class UpdateProjectProposalCommandHandler(ILogger<UpdateProjectProposalCommandHandler> logger,IUserContext userContext,
    IProjectProposalRepository projectProposalRepository, IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<UpdateProjectProposalCommand>
{
    public async Task Handle(UpdateProjectProposalCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating ProjectProposal with id: {ProjectProposalId} with {@UpdatedProjectProposal}", request.Id, request);
        var projectProposal = await projectProposalRepository.GetByIdAsync(request.Id);
        if (projectProposal is null)
            throw new NotFoundException(nameof(ProjectProposal), request.Id.ToString());

        var currentUser = userContext.GetCurrentUser()
                     ?? throw new UnAuthorizedAccessException("User not authenticated.");

        if (projectProposal.CreatorId != currentUser.Id)
            throw new BusinessRuleException("You are not allowed to update this Proposal.", 403);

        mapper.Map(request, projectProposal);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
