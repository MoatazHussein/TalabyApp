using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Users.PolicyViolations.Commands.ReviewPolicyViolation;
using Talaby.Application.Features.Users.PolicyViolations.Queries.GetPolicyViolations;
using Talaby.Domain.Constants;

namespace Talaby.API.Controllers.Admin;

[Authorize(Roles = UserRoles.Admin)]
[Route("api/admin/policy-violations")]
public sealed class AdminPolicyViolationsController(IMediator mediator) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPolicyViolations(
        [FromQuery] GetPolicyViolationsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);

        return OkResponse(result);
    }

    [HttpPatch("{violationId:guid}/review")]
    public async Task<IActionResult> ReviewPolicyViolation(
        [FromRoute] Guid violationId,
        [FromBody] ReviewPolicyViolationCommand command,
        CancellationToken cancellationToken)
    {
        command.ViolationId = violationId;

        await mediator.Send(command, cancellationToken);

        return OkResponse("Policy violation reviewed successfully");
    }
}
