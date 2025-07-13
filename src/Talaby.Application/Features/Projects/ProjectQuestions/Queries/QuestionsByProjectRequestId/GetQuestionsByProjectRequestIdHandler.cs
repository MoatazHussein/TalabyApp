using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Common.Interfaces;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;

public class GetQuestionsByProjectRequestIdHandler(IProjectQuestionReadRepository repository, ICommercialRegisterNumberMasker mask)
        : IRequestHandler<GetQuestionsByProjectRequestIdQuery, PagedResult<ProjectQuestionListItemDto>>
{
    public async Task<PagedResult<ProjectQuestionListItemDto>> Handle(
        GetQuestionsByProjectRequestIdQuery request,
        CancellationToken cancellationToken)
    {

        var result = await repository.GetPagedQuestionsAsync(
            request.ProjectRequestId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        foreach (var reply in result.Items)
        {
            reply.CreatorCommercialRegisterNumber =
                mask.Mask(reply.CreatorCommercialRegisterNumber);
        }

        return result;
    }
}
