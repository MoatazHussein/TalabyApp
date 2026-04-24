namespace Talaby.Application.Features.Payments.Contracts;

/// <summary>
/// Validates the authenticity of an incoming Tap payment webhook request
/// by verifying its HMAC hashstring against the configured secret key.
/// </summary>
public interface ITapWebhookValidator
{
    bool IsValid(string rawPayload, string receivedHashstring);
}
