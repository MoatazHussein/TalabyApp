namespace Talaby.Application.Features.Users.Services;

public interface IUserActionGuard
{
    Task EnsureCanCreateProjectAsync(Guid userId, CancellationToken cancellationToken);
    Task EnsureCanCreateProposalAsync(Guid userId, CancellationToken cancellationToken);
    Task EnsureCanCancelAcceptedWorkAsync(Guid userId, CancellationToken cancellationToken);
}
