using MediatR;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Commands.DeleteProjectQuestion;

public class DeleteProjectQuestionCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
