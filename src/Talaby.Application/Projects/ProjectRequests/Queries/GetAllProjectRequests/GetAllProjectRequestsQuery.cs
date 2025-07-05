using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Projects.ProjectRequests.Dtos;
using Talaby.Domain.Constants;

namespace Talaby.Application.Projects.ProjectRequests.Queries.GetAllProjectRequests;

public class GetAllProjectRequestsQuery : IRequest<PagedResult<ProjectRequestDto>>
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}