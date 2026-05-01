using MediatR;
using Talaby.Application.Features.Users.Services;

namespace Talaby.Application.Features.Users.Commands.DisableUser;

public sealed class DisableUserCommandHandler(IUserStatusService userStatusService)
    : IRequestHandler<DisableUserCommand>
{
    public async Task Handle(DisableUserCommand request, CancellationToken cancellationToken)
    {
        await userStatusService.DisableAsync(
            request.UserId,
            request.DisabledUntil,
            cancellationToken);
    }
}
