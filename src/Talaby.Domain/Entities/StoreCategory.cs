using Talaby.Domain.Entities.Projects;

namespace Talaby.Domain.Entities;

public class StoreCategory
{
    public int Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? Description { get; set; } 
    public string? ImageUrl { get; set; }

    //public ICollection<ProjectRequest> ProjectRequests { get; set; } = new List<ProjectRequest>();

}
