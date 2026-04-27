using MediatR;
using Talaby.Application.Features.Projects.Dtos;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;

public record GetRepliesByProposalIdQuery(
    Guid ProposalId,
    int PageNumber,
    int PageSize,
    string? SortBy = null,
    SortDirection? SortDirection = null
) : IRequest<ProposalWithRepliesDto>;
