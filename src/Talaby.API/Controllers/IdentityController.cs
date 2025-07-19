using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Users.Commands.AssignUserRole;
using Talaby.Application.Features.Users.Commands.ConfirmEmail;
using Talaby.Application.Features.Users.Commands.ForgotPassword;
using Talaby.Application.Features.Users.Commands.Login;
using Talaby.Application.Features.Users.Commands.RegisterClient;
using Talaby.Application.Features.Users.Commands.RegisterStore;
using Talaby.Application.Features.Users.Commands.ResetPassword;
using Talaby.Application.Features.Users.Commands.UnassignUserRole;
using Talaby.Application.Features.Users.Commands.UpdateUser;
using Talaby.Application.Features.Users.Queries.GetAllUsers;
using Talaby.Application.Features.Users.Queries.GetCurrentUser;
using Talaby.Domain.Constants;

namespace Talaby.API.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController(IMediator mediator, IConfiguration configuration) : ControllerBase
{


    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }

    [Authorize]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("users/me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await mediator.Send(new GetCurrentUserQuery(User));
        return Ok(result);
    }


    [HttpPost("register/client")]
    public async Task<IActionResult> RegisterClient(RegisterClientCommand command)
    {
        await mediator.Send(command);
        return Ok("Client registered successfully.");
    }

    [HttpPost("register/store")]
    public async Task<IActionResult> RegisterStore(RegisterStoreCommand command)
    {
        await mediator.Send(command);
        return Ok("Store registered successfully.");
    }

    [HttpPost("userRole")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AssignUserRole(AssignUserRoleCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("userRole")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UnassignedUserRole(UnassignUserRoleCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await mediator.Send(command);
        return Ok("If an account exists with that email, a reset link was sent.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var success = await mediator.Send(command);
        if (!success)
            return BadRequest("Invalid token or user not found.");

        return Ok("Password has been reset successfully.");
    }

    [HttpGet("reset-password")]
    public IActionResult ShowResetPasswordPage([FromQuery] string email, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            return BadRequest("Email confirmation failed.");


        return Redirect($"{configuration["App:FrontendBaseUrl"]}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}");
    }



    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            return BadRequest("Invalid email or token.");

        var command = new ConfirmEmailCommand
        {
            Email = email,
            Token = token
        };

        var result = await mediator.Send(command);

        if (!result)
            return BadRequest("Email confirmation failed.");

        //return Ok("Email confirmed successfully.");
        return Redirect($"{configuration["App:FrontendBaseUrl"]}/successful_verification?email={email}&token={token}");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }


    [HttpPatch("users")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
    {
        var result = await mediator.Send(command);

        if (!result)
            return BadRequest("Update User Data Failed");

        //return NoContent();
        return StatusCode(200, $"User Updated successfully");
    }


    [HttpPost("moyasar")]
    public IActionResult MoyasarCallback([FromBody] object payment)
    {
        //Console.WriteLine($"Received payment: {JsonConvert.SerializeObject(payment)}");
        return Ok(new { message = "Callback received" });
    }



}