using MediatR;

namespace Talaby.Application.Projects.ProjectProposals.Commands.DeleteProjectProposal;

public class DeleteProjectProposalCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
