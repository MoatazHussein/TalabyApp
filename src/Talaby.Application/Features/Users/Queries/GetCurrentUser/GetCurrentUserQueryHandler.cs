using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Dtos;
using Talaby.Domain.Entities;
using Talaby.Domain.Exceptions;

namespace Talaby.Application.Features.Users.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITimeZoneConverter _timeZoneConverter;

        public GetCurrentUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper, ITimeZoneConverter timeZoneConverter)
        {
            _userManager = userManager;
            _mapper = mapper;
            _timeZoneConverter = timeZoneConverter;
        }

        public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(request.User);
            if (user == null)
                throw new UnAuthorizedAccessException("User is not authenticated.");

            // Include related data like Category
            user = await _userManager.Users
                .Include(u => u.StoreCategory)
                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

            var dto = _mapper.Map<UserDto>(user);
            dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return _timeZoneConverter.ConvertUtcToLocal(dto);
        }
    }

}
