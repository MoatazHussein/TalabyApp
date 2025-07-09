namespace Talaby.Domain.Entities.Projects;

public class ProjectQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectRequestId { get; set; }
    public ProjectRequest? ProjectRequest { get; set; }

    public string Content { get; set; } = default!;
    public Guid CreatorId { get; set; }
    public AppUser? Creator { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuestionReply> Replies { get; set; } = new List<QuestionReply>();
}
