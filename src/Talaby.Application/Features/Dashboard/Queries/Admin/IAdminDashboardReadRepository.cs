namespace Talaby.Application.Features.Dashboard.Queries.Admin;

public interface IAdminDashboardReadRepository
{
    Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
}
