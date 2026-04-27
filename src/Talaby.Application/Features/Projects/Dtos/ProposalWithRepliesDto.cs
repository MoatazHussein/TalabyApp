using Talaby.Application.Common;
using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Projects.Dtos;

public class ProposalWithRepliesDto
{
    public Guid ProposalId { get; set; }
    public Guid ProposalCreatorId { get; set; }
    public Guid ProjectRequestId { get; set; }
    public Guid ProjectRequestCreatorId { get; set; }
    public string ProjectRequestCreatorEmail { get; set; } = default!;
    public string ProposalContent { get; set; } = default!;
    public ProjectProposalStatus ProposalStatus { get; set; }

    public PagedResult<ProposalReplyDto> Replies { get; set; } = default!;
}
