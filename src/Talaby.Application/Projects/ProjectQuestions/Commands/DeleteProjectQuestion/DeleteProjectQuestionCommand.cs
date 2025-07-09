using MediatR;

namespace Talaby.Application.Projects.ProjectQuestions.Commands.DeleteProjectQuestion;

public class DeleteProjectQuestionCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
