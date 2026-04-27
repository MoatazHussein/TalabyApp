using MediatR;
using Talaby.Application.Common;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;

public record GetProposalsByProjectRequestIdQuery(
    Guid ProjectRequestId,
    int PageNumber,
    int PageSize,
    string? SortBy = null,
    SortDirection? SortDirection = null
) : IRequest<PagedResult<ProjectProposalListItemDto>>;
