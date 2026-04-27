using System.Linq.Expressions;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Domain.Repositories.Projects;

public interface IProjectRequestRepository
{
    Task<IEnumerable<ProjectRequest>> GetAllAsync(params Expression<Func<ProjectRequest, object>>[] includes);
    Task<(IEnumerable<ProjectRequest>, int)> GetAllMatchingAsync(int storeCategoryId, string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection? sortDirection);
    Task<ProjectRequest?> GetByIdAsync(Guid id, params Expression<Func<ProjectRequest, object>>[] includes);
    Task<bool> AnyAsync(Expression<Func<ProjectRequest, bool>> predicate, CancellationToken cancellationToken);
    Task<Guid> Create(ProjectRequest entity);
    Task SaveChanges();
    Task Delete(ProjectRequest entity);

}
