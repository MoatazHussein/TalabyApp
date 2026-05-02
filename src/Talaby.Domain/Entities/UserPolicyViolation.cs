using Talaby.Domain.Enums;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Domain.Entities;

public class UserPolicyViolation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public Guid ProjectRequestId { get; set; }
    public ProjectRequest? ProjectRequest { get; set; }
    public Guid ProjectProposalId { get; set; }
    public ProjectProposal? ProjectProposal { get; set; }
    public UserPolicyViolationReason Reason { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public UserPolicyViolationReviewStatus ReviewStatus { get; set; } =
        UserPolicyViolationReviewStatus.PendingReview;
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewNote { get; set; }
}
