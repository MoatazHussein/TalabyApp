using System.Linq.Expressions;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Domain.Repositories.Projects;

public interface IProposalReplyRepository
{
    Task<ProposalReply?> GetByIdAsync(Guid id, params Expression<Func<ProposalReply, object>>[] includes);
    Task<bool> AnyAsync(Expression<Func<ProposalReply, bool>> predicate, CancellationToken cancellationToken);
    Task<Guid> Create(ProposalReply entity);
    Task Delete(ProposalReply entity);
}
