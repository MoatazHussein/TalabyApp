using MediatR;

namespace Talaby.Application.Projects.ProjectProposals.Commands.CreateProjectProposal;

public record CreateProjectProposalCommand(
Guid ProjectRequestId,
string Content,
decimal ProposedAmount
) : IRequest<Guid>;
