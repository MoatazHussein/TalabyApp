using MediatR;
using Microsoft.AspNetCore.Identity;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Users.Commands.Login;

public class LoginCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtService jwtService) : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new UnAuthorizedAccessException("Invalid credentials.");


        //if (!user.EmailConfirmed)
        //    throw new UnAuthorizedAccessException("Please confirm your email first.");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            throw new UnAuthorizedAccessException("Your account is disabled. Please contact support.");

        if (!result.Succeeded)
            throw new UnAuthorizedAccessException("Invalid credentials.");


        var token = await jwtService.GenerateTokenAsync(user);

        var roles = await userManager.GetRolesAsync(user);

        return new LoginResponseDto
        {
            Email = user.Email,
            Token = token,
            UserTypeValue = (int)user.UserType,
            UserTypeName = user.UserType.ToString(),
            Roles = roles
        };
    }
}
