using MediatR;
using Microsoft.Extensions.Logging;

namespace Talaby.Application.Features.Dashboard.Queries.Admin;

public class GetAdminDashboardQueryHandler(
    IAdminDashboardReadRepository repository,
    ILogger<GetAdminDashboardQueryHandler> logger) : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching admin dashboard data");
        return await repository.GetAdminDashboardAsync(cancellationToken);
    }
}
