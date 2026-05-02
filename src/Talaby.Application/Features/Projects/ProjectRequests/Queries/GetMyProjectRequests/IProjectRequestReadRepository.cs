using Talaby.Application.Common;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests.Dtos;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests;

public interface IProjectRequestReadRepository
{
    Task<PagedResult<MyProjectRequestDto>> GetPagedClientProjectRequestsAsync(
        Guid clientId,
        int storeCategoryId,
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken);
}
