using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Features.Users.PolicyViolations.Dtos;
using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Users.PolicyViolations.Queries.GetPolicyViolations;

public sealed class GetPolicyViolationsQuery : IRequest<PagedResult<UserPolicyViolationDto>>
{
    public Guid? UserId { get; set; }
    public UserPolicyViolationReviewStatus? ReviewStatus { get; set; }
    public UserPolicyViolationReason? Reason { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
