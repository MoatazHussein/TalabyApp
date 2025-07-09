namespace Talaby.Domain.Entities.Projects;

public class QuestionReply
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProjectQuestionId { get; set; }
    public ProjectQuestion ProjectQuestion { get; set; } = default!;

    public string Content { get; set; } = default!;

    public Guid CreatorId { get; set; }
    public AppUser? Creator { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
