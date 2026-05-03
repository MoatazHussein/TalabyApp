using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectProposalRepository(TalabyDbContext dbContext)
: IProjectProposalRepository
{
    public async Task<ProjectProposal?> GetByIdAsync(Guid id, params Expression<Func<ProjectProposal, object>>[] includes)
    {
        var query = dbContext.ProjectProposals.AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);

    }
    public async Task<IEnumerable<ProjectProposal>> GetAllAsync(Expression<Func<ProjectProposal, bool>> predicate, CancellationToken cancellationToken)
    {
      return await dbContext.ProjectProposals
            .Where(predicate)
            .ToListAsync(cancellationToken);   
    }

    public async Task<bool> AnyAsync(Expression<Func<ProjectProposal, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbContext.ProjectProposals.AnyAsync(predicate, cancellationToken);
    }

    public Task<Guid> Create(ProjectProposal entity)
    {
        dbContext.ProjectProposals.Add(entity);
        return Task.FromResult(entity.Id);
    }

    public Task Delete(ProjectProposal entity)
    {
        dbContext.Remove(entity);
        return Task.CompletedTask;
    }

  

}
