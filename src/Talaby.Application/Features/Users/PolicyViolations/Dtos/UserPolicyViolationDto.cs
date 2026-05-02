using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Users.PolicyViolations.Dtos;

public sealed class UserPolicyViolationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? UserFullName { get; set; }
    public Guid ProjectRequestId { get; set; }
    public string? ProjectRequestTitle { get; set; }
    public Guid ProjectProposalId { get; set; }
    public UserPolicyViolationReason Reason { get; set; }
    public string ReasonName { get; set; } = default!;
    public DateTime OccurredAt { get; set; }
    public UserPolicyViolationReviewStatus ReviewStatus { get; set; }
    public string ReviewStatusName { get; set; } = default!;
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
}
