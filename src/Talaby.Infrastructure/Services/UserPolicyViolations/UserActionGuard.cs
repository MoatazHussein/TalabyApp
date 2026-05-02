using Microsoft.EntityFrameworkCore;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Exceptions;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Services.UserPolicyViolations;

public class UserActionGuard(TalabyDbContext dbContext) : IUserActionGuard
{
    private const int RestrictedActionThreshold = 3;
    private const string RestrictedActionMessage =
        "Your account is restricted from this action due to repeated accepted-work cancellations.";

    public async Task EnsureCanCreateProjectAsync(Guid userId, CancellationToken cancellationToken)
    {
        await EnsureBelowViolationThresholdAsync(userId, cancellationToken);
    }

    public async Task EnsureCanCreateProposalAsync(Guid userId, CancellationToken cancellationToken)
    {
        await EnsureBelowViolationThresholdAsync(userId, cancellationToken);
    }

    public async Task EnsureCanCancelAcceptedWorkAsync(Guid userId, CancellationToken cancellationToken)
    {
        await EnsureBelowViolationThresholdAsync(userId, cancellationToken);
    }

    private async Task EnsureBelowViolationThresholdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var violationProjectCount = await dbContext.UserPolicyViolations
            .Where(violation => violation.UserId == userId)
            .Select(violation => violation.ProjectRequestId)
            .Distinct()
            .CountAsync(cancellationToken);

        if (violationProjectCount >= RestrictedActionThreshold)
        {
            throw new BusinessRuleException(
                RestrictedActionMessage,
                403,
                "USER_ACTION_RESTRICTED");
        }
    }
}
