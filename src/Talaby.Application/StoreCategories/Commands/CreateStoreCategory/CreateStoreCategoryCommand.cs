using MediatR;

namespace Talaby.Application.StoreCategories.Commands.CreateStoreCategory;

public class CreateStoreCategoryCommand : IRequest<int>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? Description { get; set; } 
    public string? ImageUrl { get; set; } 

}
