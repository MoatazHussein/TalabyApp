using MediatR;
using Talaby.Domain.Entities;
using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Mobile { get; set; }
        public string? Location { get; set; }

        //store related fields
        public string? CommercialRegisterImageUrl { get; set; }
        public string? CommercialRegisterNumber { get; set; }
        public int? StoreCategoryId { get; set; }

    }

}
