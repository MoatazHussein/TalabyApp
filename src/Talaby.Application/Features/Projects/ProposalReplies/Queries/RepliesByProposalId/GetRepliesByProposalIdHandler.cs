using MediatR;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Projects.Dtos;

namespace Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;

public class GetRepliesByProposalIdHandler(IProposalReplyReadRepository repository, ICommercialRegisterNumberMasker mask)
        : IRequestHandler<GetRepliesByProposalIdQuery, ProposalWithRepliesDto>
{
    public async Task<ProposalWithRepliesDto> Handle(
        GetRepliesByProposalIdQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetProposalWithRepliesAsync(
      request.ProposalId,
      request.PageNumber,
      request.PageSize,
      cancellationToken);

        foreach (var reply in result.Replies.Items)
        {
            reply.CreatorCommercialRegisterNumber =
                mask.Mask(reply.CreatorCommercialRegisterNumber);
        }

        return result;
    }
}
