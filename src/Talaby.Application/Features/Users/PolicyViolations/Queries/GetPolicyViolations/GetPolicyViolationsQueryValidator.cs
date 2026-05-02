using FluentValidation;

namespace Talaby.Application.Features.Users.PolicyViolations.Queries.GetPolicyViolations;

public sealed class GetPolicyViolationsQueryValidator : AbstractValidator<GetPolicyViolationsQuery>
{
    public GetPolicyViolationsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .GreaterThan(0);

        RuleFor(query => query.ReviewStatus)
            .IsInEnum()
            .When(query => query.ReviewStatus.HasValue);

        RuleFor(query => query.Reason)
            .IsInEnum()
            .When(query => query.Reason.HasValue);
    }
}
