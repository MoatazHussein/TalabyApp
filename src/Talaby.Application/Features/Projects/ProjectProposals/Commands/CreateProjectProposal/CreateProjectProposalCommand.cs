using MediatR;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.CreateProjectProposal;

public record CreateProjectProposalCommand(
Guid ProjectRequestId,
string Content,
decimal ProposedAmount
) : IRequest<Guid>;
