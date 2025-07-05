using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Projects.Dtos;

namespace Talaby.Application.Projects.ProposalReplies.Queries.RepliesByProposalId;

public record GetRepliesByProposalIdQuery(
Guid ProposalId,
int PageNumber,
int PageSize
) : IRequest<PagedResult<ProposalReplyDto>>;
