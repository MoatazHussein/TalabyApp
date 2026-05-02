using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Users.Services;

namespace Talaby.Application.Features.Dashboard.Queries.Client;

public class GetClientDashboardQueryHandler(
    IClientDashboardReadRepository repository,
    IUserContext userContext,
    ITimeZoneConverter timeZoneConverter,
    ILogger<GetClientDashboardQueryHandler> logger) : IRequestHandler<GetClientDashboardQuery, ClientDashboardDto>
{
    public async Task<ClientDashboardDto> Handle(GetClientDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("Fetching client dashboard data for user {UserId}", currentUser.Id);
        var result = await repository.GetClientDashboardAsync(currentUser.Id, cancellationToken);
        return timeZoneConverter.ConvertUtcToLocal(result);
    }
}
