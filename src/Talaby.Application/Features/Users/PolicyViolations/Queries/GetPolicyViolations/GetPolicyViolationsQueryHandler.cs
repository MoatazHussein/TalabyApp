using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.PolicyViolations.Dtos;
using Talaby.Application.Features.Users.Services;

namespace Talaby.Application.Features.Users.PolicyViolations.Queries.GetPolicyViolations;

public sealed class GetPolicyViolationsQueryHandler(
    IUserPolicyViolationService userPolicyViolationService,
    ITimeZoneConverter timeZoneConverter)
    : IRequestHandler<GetPolicyViolationsQuery, PagedResult<UserPolicyViolationDto>>
{
    public async Task<PagedResult<UserPolicyViolationDto>> Handle(
        GetPolicyViolationsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await userPolicyViolationService.GetViolationsAsync(
            request.UserId,
            request.ReviewStatus,
            request.Reason,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return timeZoneConverter.ConvertUtcToLocal(result);
    }
}
