using System.Linq.Expressions;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Domain.Repositories.Projects;

public interface IProjectQuestionRepository
{
    Task<ProjectQuestion?> GetByIdAsync(Guid id, params Expression<Func<ProjectQuestion, object>>[] includes);
    Task<bool> AnyAsync(Expression<Func<ProjectQuestion, bool>> predicate, CancellationToken cancellationToken);
    Task<Guid> Create(ProjectQuestion entity);
    Task Delete(ProjectQuestion entity);
}
