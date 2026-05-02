using MediatR;
using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Users.PolicyViolations.Commands.ReviewPolicyViolation;

public sealed class ReviewPolicyViolationCommand : IRequest
{
    public Guid ViolationId { get; set; }
    public UserPolicyViolationReviewStatus ReviewStatus { get; set; }
    public string? ReviewNote { get; set; }
}
