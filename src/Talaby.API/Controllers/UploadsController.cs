using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Talaby.Application.Features.Uploads.Commands.UploadFile;
using Talaby.Application.Features.Uploads.Commands.UploadImage;
using Talaby.API.Contracts.Uploads;

namespace Talaby.API.Controllers;

[Authorize]
[Route("api/uploads")]
public class UploadsController(IMediator mediator) : BaseApiController
{
    [HttpPost("images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        var url = await mediator.Send(new UploadImageCommand(file), cancellationToken);
        return OkResponse(new UploadResponse(url), "Uploaded successfully");
    }

    [HttpPost("files")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        var url = await mediator.Send(new UploadFileCommand(file), cancellationToken);
        return OkResponse(new UploadResponse(url), "Uploaded successfully");
    }
}

