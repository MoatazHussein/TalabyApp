using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Projects.Dtos;

namespace Talaby.Application.Projects.QuestionReplies.Queries.RepliesByQuestionId
{
    public class GetRepliesByQuestionIdHandler(IQuestionReplyReadRepository repository)
            : IRequestHandler<GetRepliesByQuestionIdQuery, PagedResult<QuestionReplyDto>>
    {
        public async Task<PagedResult<QuestionReplyDto>> Handle(
            GetRepliesByQuestionIdQuery request,
            CancellationToken cancellationToken)
        {
            return await repository.GetPagedRepliesAsync(
                request.QuestionId,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }

}
