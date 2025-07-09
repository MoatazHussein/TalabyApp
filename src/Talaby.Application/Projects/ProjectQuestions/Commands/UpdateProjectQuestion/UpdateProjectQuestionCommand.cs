using MediatR;
namespace Talaby.Application.Projects.ProjectQuestions.Commands.UpdateProjectQuestion;

public class UpdateProjectQuestionCommand : IRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = default!;
}