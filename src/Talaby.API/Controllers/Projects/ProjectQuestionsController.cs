using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Projects.ProjectQuestions.Commands.CreateProjectQuestion;
using Talaby.Application.Features.Projects.ProjectQuestions.Commands.DeleteProjectQuestion;
using Talaby.Application.Features.Projects.ProjectQuestions.Commands.UpdateProjectQuestion;
using Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;
namespace Talaby.API.Controllers.Projects;

[Authorize]
[Route("api/project-questions")]
public class ProjectQuestionsController(IMediator mediator) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectQuestionCommand command)
    {
        var id = await mediator.Send(command);
        return OkResponse("Created Successfully");
    }

    [HttpPatch()]
    public async Task<IActionResult> UpdateProjectQuestion(UpdateProjectQuestionCommand command)
    {
        await mediator.Send(command);

        return OkResponse("Updated successfully");
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectQuestion([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteProjectQuestionCommand(id));

        return OkResponse("Deleted successfully");
    }

    [HttpGet("{id}/replies")]
    public async Task<IActionResult> GetReplies(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await mediator.Send(new GetRepliesByQuestionIdQuery(id, page, pageSize));
        return OkResponse(result);
    }


}

