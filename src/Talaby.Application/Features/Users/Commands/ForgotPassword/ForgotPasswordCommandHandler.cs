using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Entities;

namespace Talaby.Application.Features.Users.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;

        public ForgotPasswordCommandHandler(
            UserManager<AppUser> userManager,
            IMailService mailService,
            IConfiguration config)
        {
            _userManager = userManager;
            _mailService = mailService;
            _config = config;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetUrl = $"{_config["App:ResetPasswordUrl"]}?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

            var emailBody = $@"
            <p>Hi,</p>
            <p>You requested to reset your password. Click the link below:</p>
            <p><a href='{resetUrl}'>Reset your password</a></p>
            <p>If you didn’t request this, just ignore this email.</p>";

            await _mailService.SendEmailAsync(user.Email!, "Password Reset Request", emailBody, null);

            return true;
        }
    }

}
