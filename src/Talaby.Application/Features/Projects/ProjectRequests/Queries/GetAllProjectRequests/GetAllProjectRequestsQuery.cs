using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.Dtos;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetAllProjectRequests;

public class GetAllProjectRequestsQuery : IRequest<PagedResult<ProjectRequestDto>>
{
    public int StoreCategoryId { get; set; }
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
}
