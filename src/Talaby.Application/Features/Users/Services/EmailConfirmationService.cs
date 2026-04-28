using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Entities;

namespace Talaby.Application.Features.Users.Services;

public class EmailConfirmationService(
    UserManager<AppUser> userManager,
    IMailService mailService,
    IConfiguration config) : IEmailConfirmationService
{
    public async Task SendConfirmationEmailAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmUrl = $"{config["App:ConfirmEmailApiUrl"]}?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(token)}";

        var emailBody = $"<p>Please confirm your email by clicking <a href='{confirmUrl}'>here</a>.</p>";

        await mailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody, null);
    }
}
