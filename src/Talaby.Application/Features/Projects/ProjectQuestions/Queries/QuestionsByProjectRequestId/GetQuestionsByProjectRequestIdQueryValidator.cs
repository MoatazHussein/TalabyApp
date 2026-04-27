using FluentValidation;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;

public class GetQuestionsByProjectRequestIdQueryValidator : AbstractValidator<GetQuestionsByProjectRequestIdQuery>
{
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(ProjectQuestion.CreatedAt)
    ];

    public GetQuestionsByProjectRequestIdQueryValidator()
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
