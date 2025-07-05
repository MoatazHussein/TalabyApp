using MediatR;
using Microsoft.AspNetCore.Identity;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Users.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly UserManager<AppUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException(nameof(AppUser), request.Email.ToString());

            if (user.EmailConfirmed)
            {
                throw new BusinessRuleException("Email already confirmed", 200, "EMAIL_ALREADY_CONFIRMED");
            }

            var result = await _userManager.ConfirmEmailAsync(user, Uri.UnescapeDataString(request.Token));

            return result.Succeeded;

        }
    }

}
