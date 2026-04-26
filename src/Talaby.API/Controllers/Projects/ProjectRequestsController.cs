using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Payments.Commands.CreateProjectCommissionCheckout;
using Talaby.Application.Features.Payments.Commands.SyncTapPaymentStatus;
using Talaby.Application.Features.Payments.Queries.VerifyProjectCommissionPayment;
using Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;
using Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.CreateProjectRequest;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.DeleteProjectRequest;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.MarkProjectRequestAsDone;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequest;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequestStatus;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.Dtos;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetAllProjectRequests;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetProjectRequestById;

namespace Talaby.API.Controllers.Projects;

[Authorize]
[Route("api/project-requests")]
public class ProjectRequestsController(IMediator mediator) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectRequestDto>>> GetAll([FromQuery] GetAllProjectRequestsQuery query)
    {
        var projectRequests = await mediator.Send(query);
        return OkResponse(projectRequests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectRequestDto?>> GetById([FromRoute] Guid id)
    {
        var projectRequest = await mediator.Send(new GetProjectRequestByIdQuery(id));
        return OkResponse(projectRequest);
    }

    [HttpGet("{id}/proposals")]
    public async Task<IActionResult> GetProposals(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetProposalsByProjectRequestIdQuery(id, page, pageSize));
        return OkResponse(result);
    }


    [HttpGet("{id}/questions")]
    public async Task<IActionResult> GetQuestions(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetQuestionsByProjectRequestIdQuery(id, page, pageSize));
        return OkResponse(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequestCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedResponse(nameof(GetById), new { id }, id);
    }


    [HttpPatch()]
    public async Task<IActionResult> UpdateProjectRequest(UpdateProjectRequestCommand command)
    {
        await mediator.Send(command);

        return OkResponse("Updated successfully");
    }

    [HttpPatch("status")]
    public async Task<IActionResult> UpdateProjectRequestStatus(UpdateProjectRequestStatusCommand command)
    {
        await mediator.Send(command);

        return OkResponse("Updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectRequest([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteProjectRequestCommand(id));

        return OkResponse("Deleted successfully");
    }

    [HttpPatch("{id}/mark-done")]
    public async Task<IActionResult> MarkAsDone([FromRoute] Guid id)
    {
        await mediator.Send(new MarkProjectRequestAsDoneCommand(id));
        return OkResponse("Project marked as done. Awaiting commission payment.");
    }

    [HttpPost("{id}/commission-payment/checkout")]
    public async Task<ActionResult<CreateProjectCommissionCheckoutResponse>> InitiateCommissionCheckout(
        [FromRoute] Guid id)
    {
        var result = await mediator.Send(new CreateProjectCommissionCheckoutCommand(id));
        return OkResponse(result);
    }

    [HttpGet("{id}/commission-payment/verify")]
    public async Task<ActionResult<VerifyProjectCommissionPaymentResponse>> VerifyCommissionPayment(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new SyncTapPaymentStatusCommand(id), cancellationToken);

        var result = await mediator.Send(
            new VerifyProjectCommissionPaymentQuery(id), cancellationToken);
        return OkResponse(result);
    }

}

