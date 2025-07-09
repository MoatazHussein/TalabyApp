using MediatR;

namespace Talaby.Application.Projects.ProjectQuestions.Commands.CreateProjectQuestion;

public record CreateProjectQuestionCommand(
Guid ProjectRequestId,
string Content
) : IRequest<Guid>;
