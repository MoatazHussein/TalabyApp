using System.Linq.Expressions;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Domain.Repositories.Projects;

public interface IQuestionReplyRepository
{
    Task<QuestionReply?> GetByIdAsync(Guid id, params Expression<Func<QuestionReply, object>>[] includes);
    Task<bool> AnyAsync(Expression<Func<QuestionReply, bool>> predicate, CancellationToken cancellationToken);
    Task<Guid> Create(QuestionReply entity);
    Task SaveChanges();
    Task Delete(QuestionReply entity);
}
