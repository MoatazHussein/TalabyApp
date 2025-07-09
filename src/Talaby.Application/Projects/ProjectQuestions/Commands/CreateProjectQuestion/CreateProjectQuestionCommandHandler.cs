using MediatR;
using Talaby.Application.Users;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Projects.ProjectQuestions.Commands.CreateProjectQuestion;

public class CreateProjectQuestionCommandHandler(
    IProjectQuestionRepository repository,
    IUserContext userContext) : IRequestHandler<CreateProjectQuestionCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = new ProjectQuestion
        {
            Id = Guid.NewGuid(),
            ProjectRequestId = request.ProjectRequestId,
            Content = request.Content,
            CreatorId = userContext.GetCurrentUser().Id,
            CreatedAt = DateTime.UtcNow
        };

        await repository.Create(question);
        return question.Id;
    }
}
