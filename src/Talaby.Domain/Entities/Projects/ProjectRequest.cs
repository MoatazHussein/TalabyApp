namespace Talaby.Domain.Entities.Projects;

public class ProjectRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public decimal MinBudget { get; set; }
    public decimal MaxBudget { get; set; }
    public int StoreCategoryId { get; set; }
    public StoreCategory StoreCategory { get; set; } = default!;

    public Guid CreatorId { get; set; }
    public AppUser? Creator { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProjectProposal> Proposals { get; set; } = new List<ProjectProposal>();
    public ICollection<ProjectQuestion> Questions { get; set; } = new List<ProjectQuestion>();
}
