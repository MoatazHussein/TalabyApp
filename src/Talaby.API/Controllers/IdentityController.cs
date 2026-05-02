using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Talaby.Application.Features.Users.Commands.AssignUserRole;
using Talaby.Application.Features.Users.Commands.ActivateUser;
using Talaby.Application.Features.Users.Commands.ConfirmEmail;
using Talaby.Application.Features.Users.Commands.DisableUser;
using Talaby.Application.Features.Users.Commands.ForgotPassword;
using Talaby.Application.Features.Users.Commands.Login;
using Talaby.Application.Features.Users.Commands.RegisterClient;
using Talaby.Application.Features.Users.Commands.RegisterStore;
using Talaby.Application.Features.Users.Commands.ResendEmailConfirmation;
using Talaby.Application.Features.Users.Commands.ResetPassword;
using Talaby.Application.Features.Users.Commands.UnassignUserRole;
using Talaby.Application.Features.Users.Commands.UpdateUser;
using Talaby.Application.Features.Users.Queries.GetAllUsers;
using Talaby.Application.Features.Users.Queries.GetCurrentUser;
using Talaby.Domain.Constants;
using Talaby.API.Contracts;

namespace Talaby.API.Controllers;

[Route("api/identity")]
public class IdentityController(IMediator mediator, IConfiguration configuration) : BaseApiController
{


    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await mediator.Send(new GetAllUsersQuery());
        return OkResponse(result);
    }

    [Authorize]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("users/me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await mediator.Send(new GetCurrentUserQuery(User));
        return OkResponse(result);
    }


    [HttpPost("register/client")]
    public async Task<IActionResult> RegisterClient(RegisterClientCommand command)
    {
        await mediator.Send(command);
        return OkResponse("Client registered successfully.");
    }

    [HttpPost("register/store")]
    public async Task<IActionResult> RegisterStore(RegisterStoreCommand command)
    {
        await mediator.Send(command);
        return OkResponse("Store registered successfully.");
    }

    //[HttpPost("userRole")]
    //[Authorize(Roles = UserRoles.Admin)]
    //public async Task<IActionResult> AssignUserRole(AssignUserRoleCommand command)
    //{
    //    await mediator.Send(command);
    //    return OkResponse("Done");
    //}

    //[HttpDelete("userRole")]
    //[Authorize(Roles = UserRoles.Admin)]
    //public async Task<IActionResult> UnassignedUserRole(UnassignUserRoleCommand command)
    //{
    //    await mediator.Send(command);
    //    return OkResponse("Done");
    //}

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await mediator.Send(command);
        return OkResponse("If an account exists with that email, a reset link was sent.");
    }

    [HttpPost("confirm-email/resend")]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationCommand command)
    {
        await mediator.Send(command);
        return OkResponse("If an account exists and is not confirmed, a confirmation email was sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var success = await mediator.Send(command);
        if (!success)
            return BadRequestResponse("Invalid token or user not found.");

        return OkResponse("Password has been reset successfully.");
    }

    [HttpGet("reset-password")]
    public IActionResult ShowResetPasswordPage([FromQuery] string email, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            return BadRequestResponse("Email confirmation failed.");


        return Redirect($"{configuration["App:FrontendBaseUrl"]}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}");
    }



    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            return BadRequestResponse("Invalid email or token.");

        var command = new ConfirmEmailCommand
        {
            Email = email,
            Token = token
        };

        var result = await mediator.Send(command);

        if (!result)
            return BadRequestResponse("Email confirmation failed.");

        return Redirect($"{configuration["App:FrontendBaseUrl"]}/successful_verification?email={email}&token={token}");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);
        return OkResponse(result);
    }


    [HttpPatch("users")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
    {
        var result = await mediator.Send(command);

        if (!result)
            return BadRequestResponse("Update User Data Failed");

        return OkResponse("User Updated successfully");
    }

    //[HttpPatch("users/{id}/disable")]
    //[Authorize(Roles = UserRoles.Admin)]
    //public async Task<IActionResult> DisableUser(
    //    [FromRoute] Guid id,
    //    [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] DisableUserRequest? request,
    //    CancellationToken cancellationToken)
    //{
    //    await mediator.Send(new DisableUserCommand(id, request?.DisabledUntil), cancellationToken);
    //    return OkResponse("User disabled successfully");
    //}

    //[HttpPatch("users/{id}/activate")]
    //[Authorize(Roles = UserRoles.Admin)]
    //public async Task<IActionResult> ActivateUser(
    //    [FromRoute] Guid id,
    //    CancellationToken cancellationToken)
    //{
    //    await mediator.Send(new ActivateUserCommand(id), cancellationToken);
    //    return OkResponse("User activated successfully");
    //}


}
