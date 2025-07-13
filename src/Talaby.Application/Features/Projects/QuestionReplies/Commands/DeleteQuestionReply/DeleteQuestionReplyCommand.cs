using MediatR;

namespace Talaby.Application.Features.Projects.QuestionReplies.Commands.DeleteQuestionReply;

public class DeleteQuestionReplyCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
