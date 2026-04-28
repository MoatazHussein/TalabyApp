using MediatR;
using Microsoft.AspNetCore.Identity;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Users.Commands.RegisterClient;

public class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, Guid>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailConfirmationService _emailConfirmationService;


    public RegisterClientCommandHandler(UserManager<AppUser> userManager, IEmailConfirmationService emailConfirmationService)
    {
        _userManager = userManager;
        _emailConfirmationService = emailConfirmationService;
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
            throw new AppException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, UserRoles.Client);

        await _emailConfirmationService.SendConfirmationEmailAsync(user, cancellationToken);


        return user.Id;
    }
}
