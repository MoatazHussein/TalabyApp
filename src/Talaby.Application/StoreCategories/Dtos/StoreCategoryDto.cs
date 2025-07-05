namespace Talaby.Application.StoreCategories.Dtos;

public class StoreCategoryDto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

}

