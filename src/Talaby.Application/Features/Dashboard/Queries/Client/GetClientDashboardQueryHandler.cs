using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users;

namespace Talaby.Application.Features.Dashboard.Queries.Client;

public class GetClientDashboardQueryHandler(
    IClientDashboardReadRepository repository,
    IUserContext userContext,
    ILogger<GetClientDashboardQueryHandler> logger) : IRequestHandler<GetClientDashboardQuery, ClientDashboardDto>
{
    public async Task<ClientDashboardDto> Handle(GetClientDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("Fetching client dashboard data for user {UserId}", currentUser.Id);
        return await repository.GetClientDashboardAsync(currentUser.Id, cancellationToken);
    }
}
