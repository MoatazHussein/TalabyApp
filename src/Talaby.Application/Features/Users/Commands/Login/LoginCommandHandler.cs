using MediatR;
using Microsoft.AspNetCore.Identity;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Users.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UnAuthorizedAccessException("Invalid credentials.");


            //if (!user.EmailConfirmed)
            //    throw new UnAuthorizedAccessException("Please confirm your email first.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
                throw new UnAuthorizedAccessException("Invalid credentials.");


            var token = await _jwtService.GenerateTokenAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return new LoginResponseDto
            {
                Email = user.Email,
                Token = token,
                UserTypeValue = (int) user.UserType,
                UserTypeName = user.UserType.ToString(),
                Roles = roles
            };
        }
    }

}
