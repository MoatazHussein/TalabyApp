using FluentValidation;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests;

public class GetMyProjectRequestsQueryValidator : AbstractValidator<GetMyProjectRequestsQuery>
{
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(ProjectRequest.CreatedAt),
        nameof(ProjectRequest.Title),
        nameof(ProjectRequest.Description)
    ];

    public GetMyProjectRequestsQueryValidator()
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
