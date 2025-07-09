namespace Talaby.Application.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;

public class ProjectQuestionListItemDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string CreatorEmail { get; set; } = default!;
    public string CreatorCommercialRegisterNumber { get; set; } = default!;
    public int RepliesCount { get; set; }
}
