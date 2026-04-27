using FluentValidation;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;

public class GetRepliesByQuestionIdQueryValidator : AbstractValidator<GetRepliesByQuestionIdQuery>
{
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(QuestionReply.CreatedAt)
    ];

    public GetRepliesByQuestionIdQueryValidator()
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
