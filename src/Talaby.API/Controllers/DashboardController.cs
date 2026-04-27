using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Dashboard.Queries.Admin;
using Talaby.Application.Features.Dashboard.Queries.Client;
using Talaby.Application.Features.Dashboard.Queries.Store;
using Talaby.Domain.Constants;

namespace Talaby.API.Controllers;

[Route("api/dashboard")]
[Authorize]
public class DashboardController(IMediator mediator) : BaseApiController
{
    [HttpGet("admin")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAdminDashboard(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAdminDashboardQuery(), cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("client")]
    [Authorize(Roles = UserRoles.Client)]
    public async Task<IActionResult> GetClientDashboard(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientDashboardQuery(), cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("store")]
    [Authorize(Roles = UserRoles.Store)]
    public async Task<IActionResult> GetStoreDashboard(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetStoreDashboardQuery(), cancellationToken);
        return OkResponse(result);
    }
}
