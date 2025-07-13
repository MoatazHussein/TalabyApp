using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Users.Dtos;
using Talaby.Domain.Entities;

namespace Talaby.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResult<UserDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(UserManager<AppUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users
            .Include(u => u.StoreCategory)
            .AsQueryable();

        // Optional search
        if (!string.IsNullOrWhiteSpace(request.SearchPhrase))
        {
            query = query.Where(u => u.Email!.Contains(request.SearchPhrase));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(u => u.StoreCategory)
            .ToListAsync(cancellationToken);

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var dto = _mapper.Map<UserDto>(user);
            dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            userDtos.Add(dto);
        }


        var pagedResult = new PagedResult<UserDto>(userDtos, totalCount, request.PageSize, request.PageNumber);


        return pagedResult;
       
    }
}


