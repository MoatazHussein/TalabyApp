using MediatR;
using Talaby.Application.Common;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.MyProjectProposals;

public class GetMyProjectProposalsQuery : IRequest<PagedResult<MyProjectProposalListItemDto>>
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
}
