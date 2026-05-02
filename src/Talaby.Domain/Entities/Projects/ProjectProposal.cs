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
    public string? CancellationReason { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public Guid? CancelledByUserId { get; private set; }

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

    public void Cancel(string? cancellationReason, Guid cancelledByUserId)
    {
        if (Status == ProjectProposalStatus.Cancelled) return; 
        if (Status == ProjectProposalStatus.Rejected)
            throw new InvalidOperationException("Cannot cancel an already-rejected proposal.");
        CancellationReason = cancellationReason;
        CancelledAtUtc = DateTime.UtcNow;
        CancelledByUserId = cancelledByUserId;
        Status = ProjectProposalStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status == ProjectProposalStatus.Completed) return;
        if (Status != ProjectProposalStatus.Accepted)
            throw new InvalidOperationException($"Cannot complete a proposal with status {Status}.");
        Status = ProjectProposalStatus.Completed;
    }
}
