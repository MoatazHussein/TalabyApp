using Talaby.Application.Common;

namespace Talaby.Application.Features.Projects.Dtos;

public class QuestionWithRepliesDto
{
    public Guid QuestionId { get; set; }
    public string QuestionContent { get; set; }

    public PagedResult<QuestionReplyDto> Replies { get; set; }
}
