using System.Linq.Expressions;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Domain.Repositories.Projects;

public interface IProjectProposalRepository
{
    Task<ProjectProposal?> GetByIdAsync(Guid id, params Expression<Func<ProjectProposal, object>>[] includes);
    Task<bool> AnyAsync(Expression<Func<ProjectProposal, bool>> predicate, CancellationToken cancellationToken);
    Task<Guid> Create(ProjectProposal entity);
    Task SaveChanges();
    Task Delete(ProjectProposal entity);
}
