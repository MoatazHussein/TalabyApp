using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectRequestRepository(TalabyDbContext dbContext)
: IProjectRequestRepository
{

    public async Task<Guid> Create(ProjectRequest entity)
    {
        dbContext.ProjectRequests.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Delete(ProjectRequest entity)
    {
        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProjectRequest>> GetAllAsync(params Expression<Func<ProjectRequest, object>>[] includes)
    {
        IQueryable<ProjectRequest> query = dbContext.ProjectRequests.AsQueryable();

        // Apply all includes
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<(IEnumerable<ProjectRequest>, int)> GetAllMatchingAsync(string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = dbContext
            .ProjectRequests
            .Where(r => searchPhraseLower == null || (r.Title.ToLower().Contains(searchPhraseLower)
                                                   || r.Description.ToLower().Contains(searchPhraseLower)));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<ProjectRequest, object>>>
            {
                { nameof(ProjectRequest.Title), r => r.Title },
                { nameof(ProjectRequest.Description), r => r.Description },
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var projectRequests = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (projectRequests, totalCount);
    }

    public async Task<ProjectRequest?> GetByIdAsync(Guid id, params Expression<Func<ProjectRequest, object>>[] includes)
    {
        var query = dbContext.ProjectRequests.AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);

    }

    public async Task<bool> AnyAsync(Expression<Func<ProjectRequest, bool>> predicate, CancellationToken cancellationToken)
    {
        return await dbContext.ProjectRequests.AnyAsync(predicate, cancellationToken);
    }

    public Task SaveChanges()
     => dbContext.SaveChangesAsync();

}