namespace Talaby.Application.Features.Dashboard.Queries.Store;

public interface IStoreDashboardReadRepository
{
    Task<StoreDashboardDto> GetStoreDashboardAsync(Guid storeId, CancellationToken cancellationToken = default);
}
