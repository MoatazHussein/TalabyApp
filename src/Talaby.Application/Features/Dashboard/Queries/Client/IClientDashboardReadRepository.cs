namespace Talaby.Application.Features.Dashboard.Queries.Client;

public interface IClientDashboardReadRepository
{
    Task<ClientDashboardDto> GetClientDashboardAsync(Guid clientId, CancellationToken cancellationToken = default);
}
