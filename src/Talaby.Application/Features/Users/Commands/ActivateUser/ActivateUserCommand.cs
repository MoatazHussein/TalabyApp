using MediatR;

namespace Talaby.Application.Features.Users.Commands.ActivateUser;

public sealed record ActivateUserCommand(Guid UserId) : IRequest;
