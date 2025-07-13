using MediatR;
using Talaby.Application.Features.Projects.Dtos;

namespace Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;

public record GetRepliesByProposalIdQuery(
Guid ProposalId,
int PageNumber,
int PageSize
) : IRequest<ProposalWithRepliesDto>;
