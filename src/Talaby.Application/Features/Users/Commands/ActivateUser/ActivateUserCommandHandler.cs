using MediatR;
using Talaby.Application.Features.Users.Services;

namespace Talaby.Application.Features.Users.Commands.ActivateUser;

public sealed class ActivateUserCommandHandler(IUserStatusService userStatusService)
    : IRequestHandler<ActivateUserCommand>
{
    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        await userStatusService.ActivateAsync(request.UserId, cancellationToken);
    }
}
