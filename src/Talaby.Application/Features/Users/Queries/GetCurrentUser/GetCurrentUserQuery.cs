using System.Security.Claims;
using MediatR;
using Talaby.Application.Features.Users.Dtos;

namespace Talaby.Application.Features.Users.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<UserDto>
{
    public ClaimsPrincipal User { get; set; }

    public GetCurrentUserQuery(ClaimsPrincipal user)
    {
        User = user;
    }
}
