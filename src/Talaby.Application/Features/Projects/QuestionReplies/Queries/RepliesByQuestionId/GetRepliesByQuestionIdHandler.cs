using MediatR;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Projects.Dtos;

namespace Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId
{
    public class GetRepliesByQuestionIdHandler(IQuestionReplyReadRepository repository, ICommercialRegisterNumberMasker mask,
        ITimeZoneConverter timeZoneConverter)
            : IRequestHandler<GetRepliesByQuestionIdQuery, QuestionWithRepliesDto>
    {
        public async Task<QuestionWithRepliesDto> Handle(
            GetRepliesByQuestionIdQuery request,
            CancellationToken cancellationToken)
        {

            var result = await repository.GetQuestionWithRepliesAsync(
           request.QuestionId,
           request.PageNumber,
           request.PageSize,
           cancellationToken);

            foreach (var reply in result.Replies.Items)
            {
                reply.CreatorCommercialRegisterNumber =
                    mask.Mask(reply.CreatorCommercialRegisterNumber);
            }

            return timeZoneConverter.ConvertUtcToLocal(result);
        }

    }
}
