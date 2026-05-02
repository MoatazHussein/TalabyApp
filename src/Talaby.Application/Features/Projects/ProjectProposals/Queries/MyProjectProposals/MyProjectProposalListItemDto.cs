using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.MyProjectProposals;

public class MyProjectProposalListItemDto
{
    public Guid Id { get; set; }
    public Guid ProjectRequestId { get; set; }
    public string ProjectTitle { get; set; } = default!;
    public ProjectRequestStatus ProjectStatus { get; set; }
    public int StoreCategoryId { get; set; }
    public decimal ProjectMinBudget { get; set; }
    public decimal ProjectMaxBudget { get; set; }
    public DateTime ProjectCreatedAt { get; set; }
    public string Content { get; set; } = default!;
    public decimal ProposedAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public ProjectProposalStatus Status { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public Guid? CancelledByUserId { get; set; }
    public string CreatorEmail { get; set; } = default!;
    public string CreatorCommercialRegisterNumber { get; set; } = default!;
    public int RepliesCount { get; set; }
}
