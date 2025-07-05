using MediatR;
using Talaby.Application.Common;

namespace Talaby.Application.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;

public class GetProposalsByProjectRequestIdHandler(IProjectProposalReadRepository repository)
        : IRequestHandler<GetProposalsByProjectRequestIdQuery, PagedResult<ProjectProposalListItemDto>>
{
    public async Task<PagedResult<ProjectProposalListItemDto>> Handle(
        GetProposalsByProjectRequestIdQuery request,
        CancellationToken cancellationToken)
    {
        return await repository.GetPagedProposalsAsync(
            request.ProjectRequestId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
