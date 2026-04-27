using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;

public class ProjectProposalListItemDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public decimal ProposedAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public ProjectProposalStatus Status { get; set; }
    public string CreatorEmail { get; set; } = default!;
    public string CreatorCommercialRegisterNumber { get; set; } = default!;
    public int RepliesCount { get; set; }
}
