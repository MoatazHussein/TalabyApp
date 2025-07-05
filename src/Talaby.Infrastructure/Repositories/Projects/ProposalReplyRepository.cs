using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProposalReplyRepository(TalabyDbContext dbContext)
: IProposalReplyRepository
{
    public async Task<ProposalReply?> GetByIdAsync(Guid id, params Expression<Func<ProposalReply, object>>[] includes)
    {
        var query = dbContext.ProposalReplies.AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);

    }

    public async Task<bool> AnyAsync(Expression<Func<ProposalReply, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbContext.ProposalReplies.AnyAsync(predicate, cancellationToken);
    }

    public async Task<Guid> Create(ProposalReply entity)
    {
        dbContext.ProposalReplies.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }
    public async Task Delete(ProposalReply entity)
    {
        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public Task SaveChanges() => dbContext.SaveChangesAsync();
}