using FluentValidation;

namespace Talaby.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandHandlerValidator()
    {
        RuleFor(x => x.StoreCategoryId)
           .GreaterThan(0)
           .WithMessage("Category id can't be 0");
    }
}

