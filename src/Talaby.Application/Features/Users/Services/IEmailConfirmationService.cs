using Talaby.Domain.Entities;

namespace Talaby.Application.Features.Users.Services;

public interface IEmailConfirmationService
{
    Task SendConfirmationEmailAsync(AppUser user, CancellationToken cancellationToken = default);
}
