using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests.Dtos;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Constants;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests;

public class GetMyProjectRequestsQueryHandler(
    IUserContext userContext,
    IProjectRequestReadRepository repository,
    ITimeZoneConverter timeZoneConverter)
    : IRequestHandler<GetMyProjectRequestsQuery, PagedResult<MyProjectRequestDto>>
{
    public async Task<PagedResult<MyProjectRequestDto>> Handle(
        GetMyProjectRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (!currentUser.IsInRole(UserRoles.Client))
            throw new BusinessRuleException("Only client users can view their project requests.", 403);

        var result = await repository.GetPagedClientProjectRequestsAsync(
            currentUser.Id,
            request.StoreCategoryId,
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        return timeZoneConverter.ConvertUtcToLocal(result);
    }
}
