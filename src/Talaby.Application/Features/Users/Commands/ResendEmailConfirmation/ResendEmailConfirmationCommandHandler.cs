using MediatR;
using Microsoft.AspNetCore.Identity;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities;

namespace Talaby.Application.Features.Users.Commands.ResendEmailConfirmation;

public class ResendEmailConfirmationCommandHandler(
    UserManager<AppUser> userManager,
    IEmailConfirmationService emailConfirmationService)
    : IRequestHandler<ResendEmailConfirmationCommand, bool>
{
    public async Task<bool> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || await userManager.IsEmailConfirmedAsync(user))
        {
            return true;
        }

        await emailConfirmationService.SendConfirmationEmailAsync(user, cancellationToken);

        return true;
    }
}
