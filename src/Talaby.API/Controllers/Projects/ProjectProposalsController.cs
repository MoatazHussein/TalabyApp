using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Projects.ProjectProposals.Commands.CreateProjectProposal;
using Talaby.Application.Features.Projects.ProjectProposals.Commands.DeleteProjectProposal;
using Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposal;
using Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposalStatus;
using Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;
namespace Talaby.API.Controllers.Projects;

[Authorize]
[Route("api/project-proposals")]
public class ProjectProposalsController(IMediator mediator) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectProposalCommand command)
    {
        var id = await mediator.Send(command);
        return OkResponse("Created Successfully");
    }

    [HttpPatch()]
    public async Task<IActionResult> UpdateProjectProposal(UpdateProjectProposalCommand command)
    {
        await mediator.Send(command);

        return OkResponse("Updated successfully");
    }

    [HttpPatch("status")]
    public async Task<IActionResult> UpdateProjectProposalStatus(UpdateProjectProposalStatusCommand command)
    {
        await mediator.Send(command);

        return OkResponse("Updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectProposal([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteProjectProposalCommand(id));

        return OkResponse("Deleted successfully");
    }

    [HttpGet("{id}/replies")]
    public async Task<IActionResult> GetReplies(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await mediator.Send(new GetRepliesByProposalIdQuery(id, page, pageSize));
        return OkResponse(result);
    }


}

