using Talaby.Domain.Enums;

namespace Talaby.Domain.Entities.Projects;

public class ProjectRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public decimal MinBudget { get; set; }
    public decimal MaxBudget { get; set; }
    public int StoreCategoryId { get; set; }
    public StoreCategory StoreCategory { get; set; } = default!;

    public Guid CreatorId { get; set; }
    public AppUser? Creator { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ProjectRequestStatus Status { get; private set; } = ProjectRequestStatus.Open;
    public string? CancellationReason { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public Guid? CancelledByUserId { get; private set; }

    public ICollection<ProjectProposal> Proposals { get; set; } = new List<ProjectProposal>();
    public ICollection<ProjectQuestion> Questions { get; set; } = new List<ProjectQuestion>();

    // --- Domain state-transition methods ---

    public void MarkInProgress()
    {
        if (Status != ProjectRequestStatus.Open)
            throw new InvalidOperationException($"Cannot move to InProgress from {Status}. Request must be Open.");
        Status = ProjectRequestStatus.InProgress;
    }

    /// <summary>InProgress → Open: the accepted proposal was cancelled, reopening the request.</summary>
    public void MarkOpen()
    {
        if (Status == ProjectRequestStatus.Open) return; 
        if (Status != ProjectRequestStatus.InProgress)
            throw new InvalidOperationException($"Cannot revert to Open from {Status}.");
        Status = ProjectRequestStatus.Open;
    }

    /// <summary>InProgress → AwaitingCommissionPayment: store marked the job done.</summary>
    public void MarkAwaitingCommissionPayment()
    {
        if (Status != ProjectRequestStatus.InProgress)
            throw new InvalidOperationException($"Cannot move to AwaitingCommissionPayment from {Status}. Request must be InProgress.");
        Status = ProjectRequestStatus.AwaitingCommissionPayment;
    }

    /// <summary>AwaitingCommissionPayment → Completed: commission has been paid (used in a future phase).</summary>
    public void MarkCompleted()
    {
        if (Status != ProjectRequestStatus.AwaitingCommissionPayment)
            throw new InvalidOperationException($"Cannot move to Completed from {Status}.");
        Status = ProjectRequestStatus.Completed;
    }

    /// <summary>Cancels the request from any non-terminal state.</summary>
    public void MarkCancelled(string? cancellationReason, Guid cancelledByUserId)
    {
        if (Status == ProjectRequestStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed project request.");
        if (Status == ProjectRequestStatus.Cancelled) return; 
        CancellationReason = cancellationReason;
        CancelledAtUtc = DateTime.UtcNow;
        CancelledByUserId = cancelledByUserId;
        Status = ProjectRequestStatus.Cancelled;
    }
}
