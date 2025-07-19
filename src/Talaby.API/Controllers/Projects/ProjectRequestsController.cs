using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;
using Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.CreateProjectRequest;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.DeleteProjectRequest;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequest;
using Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequestStatus;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.Dtos;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetAllProjectRequests;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetProjectRequestById;

namespace Talaby.API.Controllers.Projects;

[ApiController]
[Authorize]
[Route("api/project-requests")]
public class ProjectRequestsController (IMediator mediator) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectRequestDto>>> GetAll([FromQuery] GetAllProjectRequestsQuery query)
    {
        var projectRequests = await mediator.Send(query);
        return Ok(projectRequests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectRequestDto?>> GetById([FromRoute] Guid id)
    {
        var projectRequest = await mediator.Send(new GetProjectRequestByIdQuery(id));
        return Ok(projectRequest);
    }

    [HttpGet("{id}/proposals")]
    public async Task<IActionResult> GetProposals(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetProposalsByProjectRequestIdQuery(id, page, pageSize));
        return Ok(result);
    }


    [HttpGet("{id}/questions")]
    public async Task<IActionResult> GetQuestions(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetQuestionsByProjectRequestIdQuery(id, page, pageSize));
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequestCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }


    [HttpPatch()]
    public async Task<IActionResult> UpdateProjectRequest(UpdateProjectRequestCommand command)
    {
        await mediator.Send(command);

        return StatusCode(200, $"Updated successfully");
    }

    [HttpPatch("status")]
    public async Task<IActionResult> UpdateProjectRequestStatus(UpdateProjectRequestStatusCommand command)
    {
        await mediator.Send(command);

        return StatusCode(200, $"Updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectRequest([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteProjectRequestCommand(id));

        return StatusCode(200, $"Deleted successfully");
    }

}

