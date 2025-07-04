using System.Linq.Expressions;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities;

namespace Talaby.Domain.Repositories;

public interface IStoreCategoryRepository
{
    Task<IEnumerable<StoreCategory>> GetAllAsync();
    Task<StoreCategory?> GetByIdAsync(int id);
    Task<int> Create(StoreCategory entity);
    Task Delete(StoreCategory entity);
    Task SaveChanges();
    Task<(IEnumerable<StoreCategory>, int)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection);
    Task<bool> AnyAsync(Expression<Func<StoreCategory, bool>> predicate, CancellationToken cancellationToken);

}