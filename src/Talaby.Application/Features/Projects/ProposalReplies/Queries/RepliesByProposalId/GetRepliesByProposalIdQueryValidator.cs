using FluentValidation;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;

public class GetRepliesByProposalIdQueryValidator : AbstractValidator<GetRepliesByProposalIdQuery>
{
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(ProposalReply.CreatedAt)
    ];

    public GetRepliesByProposalIdQueryValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThan(0);

        RuleFor(r => r.PageSize)
            .GreaterThan(0);

        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}
