using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.Dtos;
using Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;
using Talaby.Domain.Constants;
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
        string? sortBy,
        SortDirection? sortDirection,
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
        {
            throw new NotFoundException(nameof(ProjectQuestion), questionId.ToString());
        }

        var query = context.QuestionReplies
            .Where(r => r.ProjectQuestionId == questionId);

        var totalCount = await query.CountAsync(cancellationToken);

        var columnsSelector = new Dictionary<string, Expression<Func<QuestionReply, object>>>
        {
            { nameof(QuestionReply.CreatedAt), r => r.CreatedAt },
        };

        var sortColumn = sortBy ?? nameof(QuestionReply.CreatedAt);
        var selectedColumn = columnsSelector.GetValueOrDefault(
            sortColumn,
            columnsSelector[nameof(QuestionReply.CreatedAt)]);

        query = (sortDirection ?? SortDirection.Descending) == SortDirection.Ascending
            ? query.OrderBy(selectedColumn).ThenBy(r => r.Id)
            : query.OrderByDescending(selectedColumn).ThenByDescending(r => r.Id);

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
