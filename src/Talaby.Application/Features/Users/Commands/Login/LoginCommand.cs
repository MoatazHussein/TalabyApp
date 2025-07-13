using MediatR;

namespace Talaby.Application.Features.Users.Commands.Login;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
