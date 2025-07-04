using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Users.Dtos;

namespace Talaby.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<PagedResult<UserDto>>
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


