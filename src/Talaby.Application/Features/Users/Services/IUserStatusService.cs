namespace Talaby.Application.Features.Users.Services;

public interface IUserStatusService
{
    Task DisableAsync(
        Guid userId,
        DateTimeOffset? disabledUntil,
        CancellationToken cancellationToken = default);

    Task ActivateAsync(Guid userId, CancellationToken cancellationToken = default);
}
