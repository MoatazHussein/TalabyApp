using MediatR;
using Talaby.Application.Users;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.ProjectProposals.Commands.CreateProjectProposal;

public class CreateProjectProposalCommandHandler(
    IProjectProposalRepository repository,
    IUserContext userContext) : IRequestHandler<CreateProjectProposalCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectProposalCommand request, CancellationToken cancellationToken)
    {
        var proposal = new ProjectProposal
        {
            Id = Guid.NewGuid(),
            ProjectRequestId = request.ProjectRequestId,
            Content = request.Content,
            ProposedAmount = request.ProposedAmount,
            CreatorId = userContext.GetCurrentUser().Id,
            CreatedAt = DateTime.UtcNow
        };

        await repository.Create(proposal);
        return proposal.Id;
    }
}
