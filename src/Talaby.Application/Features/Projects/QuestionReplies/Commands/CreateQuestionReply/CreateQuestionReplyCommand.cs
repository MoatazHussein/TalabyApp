using MediatR;

namespace Talaby.Application.Features.Projects.QuestionReplies.Commands.CreateQuestionReply;
public record CreateQuestionReplyCommand(
    Guid ProjectQuestionId,
    string Content
) : IRequest<Guid>;

