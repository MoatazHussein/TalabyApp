namespace Talaby.Application.Features.Users.Services;

public interface IUserConfirmationGuard
{
    Task EnsureCurrentUserEmailConfirmedAsync(CancellationToken cancellationToken = default);
}
