using MediatR;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Commands.CreateProjectQuestion;

public class CreateProjectQuestionCommandHandler(
    IProjectQuestionRepository repository,
    IUserContext userContext,
    IUserConfirmationGuard userConfirmationGuard,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProjectQuestionCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectQuestionCommand request, CancellationToken cancellationToken)
    {
        await userConfirmationGuard.EnsureCurrentUserEmailConfirmedAsync(cancellationToken);

        var question = new ProjectQuestion
        {
            Id = Guid.NewGuid(),
            ProjectRequestId = request.ProjectRequestId,
            Content = request.Content,
            CreatorId = userContext.GetCurrentUser().Id,
            CreatedAt = DateTime.UtcNow
        };

        await repository.Create(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return question.Id;
    }
}
