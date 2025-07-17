using Talaby.Application.Common.Interfaces;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Services.UnitOfWork;

public class EfUnitOfWork(TalabyDbContext _context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

