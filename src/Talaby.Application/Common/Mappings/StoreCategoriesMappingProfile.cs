using AutoMapper;
using Talaby.Application.StoreCategories.Commands.CreateStoreCategory;
using Talaby.Application.StoreCategories.Commands.UpdateStoreCategory;
using Talaby.Application.StoreCategories.Dtos;
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
