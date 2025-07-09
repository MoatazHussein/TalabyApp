using MediatR;

namespace Talaby.Application.Projects.QuestionReplies.Commands.CreateQuestionReply;
public record CreateQuestionReplyCommand(
    Guid ProjectQuestionId,
    string Content
) : IRequest<Guid>;

