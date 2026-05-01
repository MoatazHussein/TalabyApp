using Microsoft.AspNetCore.Identity;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Users.Services;

public class UserStatusService(UserManager<AppUser> userManager) : IUserStatusService
{
    public async Task DisableAsync(
        Guid userId,
        DateTimeOffset? disabledUntil,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException(nameof(AppUser), userId.ToString());

        var lockoutEnd = disabledUntil?.ToUniversalTime() ?? DateTimeOffset.UtcNow.AddYears(100);

        user.LockoutEnabled = true;
        user.LockoutEnd = lockoutEnd;

        await UpdateUserAsync(user);
    }

    public async Task ActivateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException(nameof(AppUser), userId.ToString());

        user.LockoutEnabled = true;
        user.LockoutEnd = null;

        await UpdateUserAsync(user);
    }

    private async Task UpdateUserAsync(AppUser user)
    {
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(error => error.Description));
            throw new AppException("Failed to update user status: " + errors);
        }
    }
}
