using FluentValidation;

namespace Talaby.Application.Features.Users.Commands.DisableUser;

public sealed class DisableUserCommandValidator : AbstractValidator<DisableUserCommand>
{
    public DisableUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage("User id is required");

        RuleFor(command => command.DisabledUntil)
            .Must(disabledUntil => disabledUntil is null || disabledUntil.Value.ToUniversalTime() > DateTimeOffset.UtcNow)
            .WithMessage("DisabledUntil must be in the future");
    }
}
