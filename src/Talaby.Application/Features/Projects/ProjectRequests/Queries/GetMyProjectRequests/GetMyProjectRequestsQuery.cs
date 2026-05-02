using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests.Dtos;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests;

public class GetMyProjectRequestsQuery : IRequest<PagedResult<MyProjectRequestDto>>
{
    public int StoreCategoryId { get; set; }
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
}
