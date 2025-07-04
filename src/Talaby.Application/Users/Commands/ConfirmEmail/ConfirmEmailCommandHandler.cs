using MediatR;
using Microsoft.AspNetCore.Identity;
using Talaby.Domain.Entities;

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
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);

            return result.Succeeded;

        }
    }

}
