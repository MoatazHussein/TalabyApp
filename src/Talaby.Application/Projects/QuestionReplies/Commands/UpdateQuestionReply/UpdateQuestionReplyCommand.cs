using MediatR;
namespace Talaby.Application.Projects.QuestionReplies.Commands.UpdateQuestionReply;

public class UpdateQuestionReplyCommand : IRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = default!;
}