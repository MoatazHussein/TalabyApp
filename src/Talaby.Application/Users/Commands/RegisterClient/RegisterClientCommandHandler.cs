using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Users.Commands.RegisterClient;

public class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, Guid>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMailService _mailService;
    private readonly IConfiguration _config;


    public RegisterClientCommandHandler(UserManager<AppUser> userManager, IMailService mailService, IConfiguration config)
    {
        _userManager = userManager;
        _mailService = mailService;
        _config = config;
    }

    public async Task<Guid> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new NotFoundException(nameof(AppUser), "Email already in use.");

        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email,
            Email = request.Email,
            Mobile = request.Mobile,
            Location = request.Location,
            UserType = UserType.Client
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, UserRoles.Client);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var confirmUrl = $"{_config["App:ConfirmEmailApiUrl"]}?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

        var emailBody = $"<p>Please confirm your email by clicking <a href='{confirmUrl}'>here</a>.</p>";

        await _mailService.SendEmailAsync(user.Email, "Confirm your email", emailBody, null);


        return user.Id;
    }
}
