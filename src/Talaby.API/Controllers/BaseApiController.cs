using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Common;

namespace Talaby.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected OkObjectResult OkResponse<T>(T data, string? message = null)
        => Ok(ApiResponse<T>.Success(data, message));

    protected OkObjectResult OkResponse(string message)
        => Ok(ApiResponse.Success(message));

    protected CreatedAtActionResult CreatedResponse<T>(string actionName, object routeValues, T data)
        => CreatedAtAction(actionName, routeValues, ApiResponse<T>.Success(data));

    protected BadRequestObjectResult BadRequestResponse(string message)
        => BadRequest(ApiResponse.Fail(message));
}
