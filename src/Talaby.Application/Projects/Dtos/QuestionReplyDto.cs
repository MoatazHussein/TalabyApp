namespace Talaby.Application.Projects.Dtos;

public class QuestionReplyDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string CreatorEmail { get; set; } = default!;
    public string CreatorCommercialRegisterNumber { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
