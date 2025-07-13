using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Projects.ProposalReplies.Commands.CreateProposalReply;
using Talaby.Application.Features.Projects.ProposalReplies.Commands.DeleteProposalReply;
using Talaby.Application.Features.Projects.ProposalReplies.Commands.UpdateProposalReply;
namespace Talaby.API.Controllers.Projects;

[ApiController]
[Authorize]
[Route("api/proposal-replies")]
public class ProposalRepliesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProposalReplyCommand command)
    {
        var id = await mediator.Send(command);
        return Ok("Created Successfully");
    }

    [HttpPatch()]
    public async Task<IActionResult> UpdateProposalReply(UpdateProposalReplyCommand command)
    {
        await mediator.Send(command);

        return StatusCode(200, $"Updated successfully");
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProposalReply([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteProposalReplyCommand(id));

        return StatusCode(200, $"Deleted successfully");
    }

}

