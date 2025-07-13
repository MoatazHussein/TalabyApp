using MediatR;
namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposal;

public class UpdateProjectProposalCommand : IRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = default!;
    public decimal ProposedAmount { get; set; }
}