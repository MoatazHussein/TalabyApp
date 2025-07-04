using System.Security.Claims;
using MediatR;
using Talaby.Application.Users.Dtos;

namespace Talaby.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<UserDto>
{
    public ClaimsPrincipal User { get; set; }

    public GetCurrentUserQuery(ClaimsPrincipal user)
    {
        User = user;
    }
}
