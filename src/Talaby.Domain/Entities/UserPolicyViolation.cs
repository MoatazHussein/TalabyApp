using Talaby.Domain.Enums;

namespace Talaby.Domain.Entities;

public class UserPolicyViolation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public Guid ProjectRequestId { get; set; }
    public Guid ProjectProposalId { get; set; }
    public UserPolicyViolationReason Reason { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
