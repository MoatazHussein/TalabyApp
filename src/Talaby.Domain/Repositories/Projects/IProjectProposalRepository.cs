using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Domain.Repositories.Projects;

public interface IProjectProposalRepository
{
    Task<ProjectProposal?> GetByIdAsync(Guid id, params Expression<Func<ProjectProposal, object>>[] includes);
    Task<IEnumerable<ProjectProposal>> GetAllAsync(Expression<Func<ProjectProposal, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<ProjectProposal, bool>> predicate, CancellationToken cancellationToken);
    Task<Guid> Create(ProjectProposal entity);
    Task Delete(ProjectProposal entity);
}
