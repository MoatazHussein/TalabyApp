using MediatR;
using Microsoft.AspNetCore.Identity;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<ResetPasswordCommand, bool>
{
    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new NotFoundException(nameof(AppUser), request.Email.ToString());

        var result = await userManager.ResetPasswordAsync(user, Uri.UnescapeDataString(request.Token), request.NewPassword);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code.Contains("InvalidToken")))
            {
                throw new BusinessRuleException(
                    "Reset token has expired. Please Try again.",
                    400,
                    "RESET_TOKEN_EXPIRED"
                );
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessRuleException($"Password reset failed: {errors}", 400, "PASSWORD_RESET_FAILED");
        }


        return result.Succeeded;
    }
}
