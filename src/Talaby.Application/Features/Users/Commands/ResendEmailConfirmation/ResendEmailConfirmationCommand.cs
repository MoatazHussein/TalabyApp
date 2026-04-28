using MediatR;

namespace Talaby.Application.Features.Users.Commands.ResendEmailConfirmation;

public class ResendEmailConfirmationCommand : IRequest<bool>
{
    public string Email { get; set; } = default!;
}
