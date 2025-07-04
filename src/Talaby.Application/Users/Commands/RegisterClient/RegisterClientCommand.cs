using MediatR;

namespace Talaby.Application.Users.Commands.RegisterClient;

public class RegisterClientCommand : IRequest<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Mobile { get; set; } 

    public string? Location { get; set; }
}
