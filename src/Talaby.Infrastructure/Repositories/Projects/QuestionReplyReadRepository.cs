using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.Dtos;
using Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class QuestionReplyReadRepository(TalabyDbContext context) : IQuestionReplyReadRepository
{
    public async Task<QuestionWithRepliesDto> GetQuestionWithRepliesAsync(
    Guid questionId,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
    {
        var question = await context.ProjectQuestions
            .Where(q => q.Id == questionId)
            .Select(q => new
            {
                q.Id,
                q.Content,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (question == null)
        throw new NotFoundException(nameof(ProjectQuestion), questionId.ToString());


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

        return new QuestionWithRepliesDto
        {
            QuestionId = question.Id,
            QuestionContent = question.Content,
            Replies = new PagedResult<QuestionReplyDto>(items, totalCount, pageSize, pageNumber)
        };

    }
}
