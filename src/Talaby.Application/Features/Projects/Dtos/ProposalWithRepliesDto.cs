using Talaby.Application.Common;

namespace Talaby.Application.Features.Projects.Dtos;

public class ProposalWithRepliesDto
{
    public Guid ProposalId { get; set; }
    public string ProposalContent { get; set; }

    public PagedResult<ProposalReplyDto> Replies { get; set; }
}
