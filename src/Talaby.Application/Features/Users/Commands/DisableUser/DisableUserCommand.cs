using MediatR;

namespace Talaby.Application.Features.Users.Commands.DisableUser;

public sealed record DisableUserCommand(Guid UserId, DateTimeOffset? DisabledUntil) : IRequest;
