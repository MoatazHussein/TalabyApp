using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Users.PolicyViolations.Dtos;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Services.UserPolicyViolations;

public class UserPolicyViolationService(
    TalabyDbContext dbContext) : IUserPolicyViolationService
{
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

    public async Task<PagedResult<UserPolicyViolationDto>> GetViolationsAsync(
        Guid? userId,
        UserPolicyViolationReviewStatus? reviewStatus,
        UserPolicyViolationReason? reason,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.UserPolicyViolations
            .AsNoTracking()
            .Include(violation => violation.User)
            .Include(violation => violation.ProjectRequest)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(violation => violation.UserId == userId.Value);
        }

        if (reviewStatus.HasValue)
        {
            query = query.Where(violation => violation.ReviewStatus == reviewStatus.Value);
        }

        if (reason.HasValue)
        {
            query = query.Where(violation => violation.Reason == reason.Value);
        }

        query = query
            .OrderByDescending(violation => violation.OccurredAtUtc)
            .ThenByDescending(violation => violation.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var violations = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var violationDtos = violations
            .Select(violation => new UserPolicyViolationDto
            {
                Id = violation.Id,
                UserId = violation.UserId,
                UserEmail = violation.User!.Email,
                UserFullName = (violation.User.FirstName + " " + violation.User.LastName).Trim(),
                ProjectRequestId = violation.ProjectRequestId,
                ProjectRequestTitle = violation.ProjectRequest!.Title,
                ProjectProposalId = violation.ProjectProposalId,
                Reason = violation.Reason,
                ReasonName = violation.Reason.ToString(),
                OccurredAt = violation.OccurredAtUtc,
                ReviewStatus = violation.ReviewStatus,
                ReviewStatusName = violation.ReviewStatus.ToString(),
                ReviewedByUserId = violation.ReviewedByUserId,
                ReviewedAt = violation.ReviewedAtUtc,
                ReviewNote = violation.ReviewNote
            })
            .ToList();

        return new PagedResult<UserPolicyViolationDto>(
            violationDtos,
            totalCount,
            pageSize,
            pageNumber);
    }

    public async Task ReviewViolationAsync(
        Guid violationId,
        UserPolicyViolationReviewStatus reviewStatus,
        string? reviewNote,
        Guid reviewedByUserId,
        CancellationToken cancellationToken = default)
    {
        if (reviewStatus is not (UserPolicyViolationReviewStatus.Confirmed
            or UserPolicyViolationReviewStatus.Waived))
        {
            throw new BusinessRuleException("Review status must be Confirmed or Waived.");
        }

        var violation = await dbContext.UserPolicyViolations
            .FirstOrDefaultAsync(
                violation => violation.Id == violationId,
                cancellationToken)
            ?? throw new NotFoundException(nameof(UserPolicyViolation), violationId.ToString());

        var reviewedAtUtc = DateTime.UtcNow;

        violation.ReviewStatus = reviewStatus;
        violation.ReviewNote = reviewNote;
        violation.ReviewedByUserId = reviewedByUserId;
        violation.ReviewedAtUtc = reviewedAtUtc;
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
    }
}
