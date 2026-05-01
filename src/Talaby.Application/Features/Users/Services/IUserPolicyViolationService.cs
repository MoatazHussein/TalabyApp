using Talaby.Domain.Entities.Projects;

namespace Talaby.Application.Features.Users.Services;

public interface IUserPolicyViolationService
{
    Task RecordAcceptedProjectCancellationAsync(
        Guid projectRequestId,
        CancellationToken cancellationToken = default);

    Task RecordAcceptedProposalCancellationAsync(
        ProjectProposal projectProposal,
        CancellationToken cancellationToken = default);
}
