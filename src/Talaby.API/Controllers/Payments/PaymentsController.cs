using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.Payments.Commands.ProcessTapCommissionWebhook;

namespace Talaby.API.Controllers.Payments;

[ApiController]
[Route("api/payments")]
public class PaymentsController(
    IMediator mediator,
    ILogger<PaymentsController> logger) : ControllerBase
{
    private const string TapHashstringHeader = "hashstring";

    /// <summary>
    /// Receives and processes Tap payment webhook notifications.
    /// The endpoint is intentionally anonymous — authenticity is verified
    /// by the hashstring HMAC signature included in the request header.
    /// Always returns 200 to prevent Tap from retrying a delivery indefinitely.
    /// </summary>
    [HttpPost("tap/webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> TapWebhook(CancellationToken cancellationToken)
    {
        // Read the raw body so we can compute the HMAC over exactly what Tap signed.
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var rawPayload = await reader.ReadToEndAsync(cancellationToken);

        var hashstring = Request.Headers[TapHashstringHeader].FirstOrDefault() ?? string.Empty;

        logger.LogDebug("Tap webhook received. PayloadLength={PayloadLength}", rawPayload.Length);

        try
        {
            await mediator.Send(
                new ProcessTapCommissionWebhookCommand(rawPayload, hashstring),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unhandled exception processing Tap webhook. PayloadLength={PayloadLength}",
                rawPayload.Length);
        }

        return Ok();
    }
}
