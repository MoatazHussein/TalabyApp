using MediatR;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.QuestionReplies.Commands.CreateQuestionReply;

public class CreateQuestionReplyCommandHandler(
    IQuestionReplyRepository repository,
    IUserContext currentUser,
    IUserConfirmationGuard userConfirmationGuard) : IRequestHandler<CreateQuestionReplyCommand, Guid>
{
    public async Task<Guid> Handle(CreateQuestionReplyCommand request, CancellationToken cancellationToken)
    {
        await userConfirmationGuard.EnsureCurrentUserEmailConfirmedAsync(cancellationToken);

        var reply = new QuestionReply
        {
            Id = Guid.NewGuid(),
            ProjectQuestionId = request.ProjectQuestionId,
            Content = request.Content,
            CreatorId = currentUser.GetCurrentUser().Id,
            CreatedAt = DateTime.UtcNow
        };

        await repository.Create(reply);
        return reply.Id;
    }
}
