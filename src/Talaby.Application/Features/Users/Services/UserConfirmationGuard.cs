using Microsoft.AspNetCore.Identity;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Users.Services;

public class UserConfirmationGuard(
    IUserContext userContext,
    UserManager<AppUser> userManager) : IUserConfirmationGuard
{
    public async Task EnsureCurrentUserEmailConfirmedAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = userContext.GetCurrentUser();

        var user = await userManager.FindByIdAsync(currentUser.Id.ToString());
        if (user is null)
        {
            throw new UnAuthorizedAccessException("User is not authenticated.");
        }

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            throw new BusinessRuleException(
                "Please confirm your email first.",
                403,
                "EMAIL_NOT_CONFIRMED");
        }
    }
}
