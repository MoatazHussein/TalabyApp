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
    public ProjectProposalStatus Status { get; private set; } = ProjectProposalStatus.Pending;

    public ICollection<ProposalReply> Replies { get; set; } = new List<ProposalReply>();

    // --- Domain state-transition methods ---

    public void Accept()
    {
        if (Status == ProjectProposalStatus.Accepted) return; 
        if (Status != ProjectProposalStatus.Pending)
            throw new InvalidOperationException($"Cannot accept a proposal with status {Status}.");
        Status = ProjectProposalStatus.Accepted;
    }

    public void Reject()
    {
        if (Status == ProjectProposalStatus.Rejected) return; 
        if (Status != ProjectProposalStatus.Pending)
            throw new InvalidOperationException($"Cannot reject a proposal with status {Status}.");
        Status = ProjectProposalStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status == ProjectProposalStatus.Cancelled) return; 
        if (Status == ProjectProposalStatus.Rejected)
            throw new InvalidOperationException("Cannot cancel an already-rejected proposal.");
        Status = ProjectProposalStatus.Cancelled;
    }
}
