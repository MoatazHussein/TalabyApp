using FluentValidation;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Projects.ProjectRequests.Queries.GetAllProjectRequests;

public class GetAllProjectRequestsQueryHandlerValidator : AbstractValidator<GetAllProjectRequestsQuery>
{
    private int[] allowPageSizes = [5, 10, 15, 30];
    private string[] allowedSortByColumnNames =[ nameof(ProjectRequest.Title),nameof(ProjectRequest.Description) ];


    public GetAllProjectRequestsQueryHandlerValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");

        //RuleFor(r => r.PageSize)
        //    .Must(value => allowPageSizes.Contains(value))
        //    .WithMessage($"Page size must be in [{string.Join(",", allowPageSizes)}]");
    }
}