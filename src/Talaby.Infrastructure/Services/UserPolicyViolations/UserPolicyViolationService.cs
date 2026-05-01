using Microsoft.EntityFrameworkCore;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Services.UserPolicyViolations;

public class UserPolicyViolationService(
    TalabyDbContext dbContext,
    IUserStatusService userStatusService) : IUserPolicyViolationService
{
    private const int AutoDisableThreshold = 3;

    public async Task RecordAcceptedProjectCancellationAsync(
        Guid projectRequestId,
        CancellationToken cancellationToken = default)
    {
        var acceptedProposal = await dbContext.ProjectProposals
            .AsNoTracking()
            .FirstOrDefaultAsync(
                proposal => proposal.ProjectRequestId == projectRequestId
                            && proposal.Status == ProjectProposalStatus.Accepted,
                cancellationToken);

        if (acceptedProposal is null)
        {
            return;
        }

        await RecordAsync(
            acceptedProposal.CreatorId,
            acceptedProposal.ProjectRequestId,
            acceptedProposal.Id,
            UserPolicyViolationReason.AcceptedProjectCancelled,
            cancellationToken);
    }

    public async Task RecordAcceptedProposalCancellationAsync(
        ProjectProposal projectProposal,
        CancellationToken cancellationToken = default)
    {
        await RecordAsync(
            projectProposal.CreatorId,
            projectProposal.ProjectRequestId,
            projectProposal.Id,
            UserPolicyViolationReason.AcceptedProposalCancelled,
            cancellationToken);
    }

    private async Task RecordAsync(
        Guid userId,
        Guid projectRequestId,
        Guid projectProposalId,
        UserPolicyViolationReason reason,
        CancellationToken cancellationToken)
    {
        var alreadyRecorded = await dbContext.UserPolicyViolations.AnyAsync(
            violation => violation.UserId == userId
                         && violation.ProjectRequestId == projectRequestId
                         && violation.Reason == reason,
            cancellationToken);

        if (alreadyRecorded)
        {
            return;
        }

        dbContext.UserPolicyViolations.Add(new UserPolicyViolation
        {
            UserId = userId,
            ProjectRequestId = projectRequestId,
            ProjectProposalId = projectProposalId,
            Reason = reason,
            OccurredAtUtc = DateTime.UtcNow
        });

        var projectAlreadyCounted = await dbContext.UserPolicyViolations.AnyAsync(
            violation => violation.UserId == userId
                         && violation.ProjectRequestId == projectRequestId,
            cancellationToken);

        var distinctProjectCount = await dbContext.UserPolicyViolations
            .Where(violation => violation.UserId == userId)
            .Select(violation => violation.ProjectRequestId)
            .Distinct()
            .CountAsync(cancellationToken);

        var newDistinctProjectCount = distinctProjectCount + (projectAlreadyCounted ? 0 : 1);

        if (newDistinctProjectCount >= AutoDisableThreshold)
        {
            await userStatusService.DisableAsync(userId, null, cancellationToken);
        }
    }
}
