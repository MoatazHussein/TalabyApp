using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Projects.QuestionReplies.Commands.CreateQuestionReply;
using Talaby.Application.Features.Projects.QuestionReplies.Commands.DeleteQuestionReply;
using Talaby.Application.Features.Projects.QuestionReplies.Commands.UpdateQuestionReply;
namespace Talaby.API.Controllers.Projects;

[ApiController]
[Authorize]
[Route("api/question-replies")]
public class QuestionRepliesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionReplyCommand command)
    {
        var id = await mediator.Send(command);
        return Ok("Created Successfully");
    }

    [HttpPatch()]
    public async Task<IActionResult> UpdateQuestionReply(UpdateQuestionReplyCommand command)
    {
        await mediator.Send(command);

        return StatusCode(200, $"Updated successfully");
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestionReply([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteQuestionReplyCommand(id));

        return StatusCode(200, $"Deleted successfully");
    }

}

