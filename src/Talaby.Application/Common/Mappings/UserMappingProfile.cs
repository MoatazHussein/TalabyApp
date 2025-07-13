using AutoMapper;
using Talaby.Application.Features.Users.Dtos;
using Talaby.Domain.Entities;

namespace Talaby.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<AppUser, UserDto>()
      .ForMember(dest => dest.Roles, opt => opt.Ignore()) // handled manually
      .ForMember(dest => dest.UserTypeValue, opt => opt.MapFrom(src => (int)src.UserType))
      .ForMember(dest => dest.UserTypeName, opt => opt.MapFrom(src => src.UserType.ToString()));
          //.ForMember(dest => dest.StoreCategory, opt => opt.MapFrom(src => src.StoreCategory));



    }
}
