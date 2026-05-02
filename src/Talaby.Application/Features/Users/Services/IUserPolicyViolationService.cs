using Talaby.Application.Common;
using Talaby.Application.Features.Users.PolicyViolations.Dtos;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Users.Services;

public interface IUserPolicyViolationService
{
    Task RecordAcceptedProjectCancellationAsync(
        Guid projectRequestId,
        CancellationToken cancellationToken = default);

    Task RecordAcceptedProposalCancellationAsync(
        ProjectProposal projectProposal,
        CancellationToken cancellationToken = default);

    Task<PagedResult<UserPolicyViolationDto>> GetViolationsAsync(
        Guid? userId,
        UserPolicyViolationReviewStatus? reviewStatus,
        UserPolicyViolationReason? reason,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task ReviewViolationAsync(
        Guid violationId,
        UserPolicyViolationReviewStatus reviewStatus,
        string? reviewNote,
        Guid reviewedByUserId,
        CancellationToken cancellationToken = default);
}
