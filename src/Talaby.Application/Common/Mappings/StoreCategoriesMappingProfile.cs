using AutoMapper;
using Talaby.Application.Features.StoreCategories.Commands.CreateStoreCategory;
using Talaby.Application.Features.StoreCategories.Commands.UpdateStoreCategory;
using Talaby.Application.Features.StoreCategories.Dtos;
using Talaby.Domain.Entities;

namespace Talaby.Application.Common.Mappings;

public class StoreCategoriesMappingProfile : Profile
{
    public StoreCategoriesMappingProfile()
    {
        CreateMap<StoreCategory, StoreCategoryDto>();
        CreateMap<CreateStoreCategoryCommand, StoreCategory>();
        CreateMap<UpdateStoreCategoryCommand, StoreCategory>();

    }
}
