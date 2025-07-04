using MediatR;

namespace Talaby.Application.Users.Commands.RegisterStore;

public class RegisterStoreCommand : IRequest<Guid>
{
    public string FirstName { get; set; } = default!;
    //public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? Mobile { get; set; }

    public string CommercialRegisterNumber { get; set; } = default!;
    public string CommercialRegisterImageUrl { get; set; } = default!;
    public int StoreCategoryId { get; set; }

    public string? Location { get; set; }
}