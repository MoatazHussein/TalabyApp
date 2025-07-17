using Talaby.Domain.Enums;

namespace Talaby.Domain.Entities.Projects;

public class ProjectProposal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectRequestId { get; set; }
    public ProjectRequest? ProjectRequest { get; set; }

    public string Content { get; set; } = default!;
    public Guid CreatorId { get; set; }
    public AppUser? Creator { get; set; }
    public decimal ProposedAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ProjectProposalStatus Status { get; set; } = ProjectProposalStatus.Pending;


    public ICollection<ProposalReply> Replies { get; set; } = new List<ProposalReply>();
}
