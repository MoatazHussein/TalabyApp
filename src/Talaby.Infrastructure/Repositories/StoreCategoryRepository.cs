using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities;
using Talaby.Domain.Repositories;
using System.Linq.Expressions;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories;

internal class StoreCategoryRepository(TalabyDbContext dbContext)
    : IStoreCategoryRepository
{

    public async Task<int> Create(StoreCategory entity)
    {
        dbContext.StoreCategories.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Delete(StoreCategory entity)
    {
        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<StoreCategory>> GetAllAsync()
    {
        var StoreCategories = await dbContext.StoreCategories.ToListAsync();
        return StoreCategories;
    }

    public async Task<(IEnumerable<StoreCategory>, int)> GetAllMatchingAsync(string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext
            .StoreCategories
            .Where(r => searchPhraseLower == null || (r.NameAr.ToLower().Contains(searchPhraseLower)
                                                   || r.NameEn.ToLower().Contains(searchPhraseLower)));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<StoreCategory, object>>>
            {
                { nameof(StoreCategory.NameAr), r => r.NameAr },
                { nameof(StoreCategory.NameEn), r => r.NameEn },
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var StoreCategories = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (StoreCategories, totalCount);
    }

    public async Task<StoreCategory?> GetByIdAsync(int id)
    {
        var StoreCategories = await dbContext.StoreCategories
            .FirstOrDefaultAsync(x => x.Id == id);

        return StoreCategories;
    }

    public async Task<bool> AnyAsync(Expression<Func<StoreCategory, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbContext.StoreCategories.AnyAsync(predicate, cancellationToken);
    }

    public Task SaveChanges()
     => dbContext.SaveChangesAsync();

}