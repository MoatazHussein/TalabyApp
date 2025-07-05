using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Projects.ProjectProposals.Commands.CreateProjectProposal;
using Talaby.Application.Projects.ProjectProposals.Commands.DeleteProjectProposal;
using Talaby.Application.Projects.ProjectProposals.Commands.UpdateProjectProposal;
using Talaby.Application.Projects.ProposalReplies.Queries.RepliesByProposalId;
namespace Talaby.API.Controllers.Projects;

[ApiController]
[Authorize]
[Route("api/project-proposals")]
public class ProjectProposalsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectProposalCommand command)
    {
        var id = await mediator.Send(command);
        return Ok("Created Successfully");
    }

    [HttpPatch()]
    public async Task<IActionResult> UpdateProjectProposal(UpdateProjectProposalCommand command)
    {
        await mediator.Send(command);

        return StatusCode(200, $"Updated successfully");
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectProposal([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteProjectProposalCommand(id));

        return StatusCode(200, $"Deleted successfully");
    }

    [HttpGet("{id}/replies")]
    public async Task<IActionResult> GetReplies(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await mediator.Send(new GetRepliesByProposalIdQuery(id, page, pageSize));
        return Ok(result);
    }


}

