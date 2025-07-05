using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Projects.Dtos;

namespace Talaby.Application.Projects.ProposalReplies.Queries.RepliesByProposalId
{
    public class GetRepliesByProposalIdHandler(IProposalReplyReadRepository repository)
            : IRequestHandler<GetRepliesByProposalIdQuery, PagedResult<ProposalReplyDto>>
    {
        public async Task<PagedResult<ProposalReplyDto>> Handle(
            GetRepliesByProposalIdQuery request,
            CancellationToken cancellationToken)
        {
            return await repository.GetPagedRepliesAsync(
                request.ProposalId,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }

}
