using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class QuestionReplyRepository(TalabyDbContext dbContext)
: IQuestionReplyRepository
{
    public async Task<QuestionReply?> GetByIdAsync(Guid id, params Expression<Func<QuestionReply, object>>[] includes)
    {
        var query = dbContext.QuestionReplies.AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);

    }

    public async Task<bool> AnyAsync(Expression<Func<QuestionReply, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbContext.QuestionReplies.AnyAsync(predicate, cancellationToken);
    }

    public Task<Guid> Create(QuestionReply entity)
    {
        dbContext.QuestionReplies.Add(entity);
        return Task.FromResult(entity.Id);
    }

    public Task Delete(QuestionReply entity)
    {
        dbContext.Remove(entity);
        return Task.CompletedTask;
    }
}
