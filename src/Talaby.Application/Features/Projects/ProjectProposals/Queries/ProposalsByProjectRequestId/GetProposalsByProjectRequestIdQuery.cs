using MediatR;
using Talaby.Application.Common;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId
{
    public record GetProposalsByProjectRequestIdQuery(
     Guid ProjectRequestId,
     int PageNumber,
     int PageSize
 ) : IRequest<PagedResult<ProjectProposalListItemDto>>;

}
