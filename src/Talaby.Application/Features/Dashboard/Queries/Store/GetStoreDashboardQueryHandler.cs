using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Users.Services;

namespace Talaby.Application.Features.Dashboard.Queries.Store;

public class GetStoreDashboardQueryHandler(
    IStoreDashboardReadRepository repository,
    IUserContext userContext,
    ILogger<GetStoreDashboardQueryHandler> logger) : IRequestHandler<GetStoreDashboardQuery, StoreDashboardDto>
{
    public async Task<StoreDashboardDto> Handle(GetStoreDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        logger.LogInformation("Fetching store dashboard data for user {UserId}", currentUser.Id);
        return await repository.GetStoreDashboardAsync(currentUser.Id, cancellationToken);
    }
}
