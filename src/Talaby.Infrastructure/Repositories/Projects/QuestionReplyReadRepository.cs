using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Projects.Dtos;
using Talaby.Application.Projects.QuestionReplies.Queries.RepliesByQuestionId;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class QuestionReplyReadRepository(TalabyDbContext context) : IQuestionReplyReadRepository
{
    public async Task<PagedResult<QuestionReplyDto>> GetPagedRepliesAsync(
        Guid questionId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.QuestionReplies
            .Where(r => r.ProjectQuestionId == questionId)
            .OrderBy(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new QuestionReplyDto
            {
                Id = r.Id,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                CreatorEmail = r.Creator.Email,
                CreatorCommercialRegisterNumber = r.Creator.CommercialRegisterNumber
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<QuestionReplyDto>(items, totalCount, pageSize, pageNumber);
    }
}
