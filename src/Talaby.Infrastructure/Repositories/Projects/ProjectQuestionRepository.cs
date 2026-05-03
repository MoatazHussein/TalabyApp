using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectQuestionRepository(TalabyDbContext dbContext)
: IProjectQuestionRepository
{
    public async Task<ProjectQuestion?> GetByIdAsync(Guid id, params Expression<Func<ProjectQuestion, object>>[] includes)
    {
        var query = dbContext.ProjectQuestions.AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);

    }

    public async Task<bool> AnyAsync(Expression<Func<ProjectQuestion, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbContext.ProjectQuestions.AnyAsync(predicate, cancellationToken);
    }

    public Task<Guid> Create(ProjectQuestion entity)
    {
        dbContext.ProjectQuestions.Add(entity);
        return Task.FromResult(entity.Id);
    }

    public Task Delete(ProjectQuestion entity)
    {
        dbContext.Remove(entity);
        return Task.CompletedTask;
    }
}
