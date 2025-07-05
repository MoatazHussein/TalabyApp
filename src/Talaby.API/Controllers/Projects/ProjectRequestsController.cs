using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Projects.ProjectRequests.Commands.CreateProjectRequest;
using Talaby.Application.Projects.ProjectRequests.Commands.DeleteProjectRequest;
using Talaby.Application.Projects.ProjectRequests.Commands.UpdateProjectRequest;
using Talaby.Application.Projects.ProjectRequests.Dtos;
using Talaby.Application.Projects.ProjectRequests.Queries.GetAllProjectRequests;
using Talaby.Application.Projects.ProjectRequests.Queries.GetProjectRequestById;

namespace Talaby.API.Controllers.Projects;

[ApiController]
[Authorize]
[Route("api/project-requests")]
public class ProjectRequestsController (IMediator mediator, IConfiguration configuration) : ControllerBase
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectRequest([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteProjectRequestCommand(id));

        return StatusCode(200, $"Deleted successfully");
    }

}

