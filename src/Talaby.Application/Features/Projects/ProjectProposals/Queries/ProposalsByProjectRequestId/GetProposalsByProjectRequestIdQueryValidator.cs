using FluentValidation;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;

public class GetProposalsByProjectRequestIdQueryValidator : AbstractValidator<GetProposalsByProjectRequestIdQuery>
{
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(ProjectProposal.CreatedAt),
        nameof(ProjectProposal.ProposedAmount)
    ];

    public GetProposalsByProjectRequestIdQueryValidator()
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
