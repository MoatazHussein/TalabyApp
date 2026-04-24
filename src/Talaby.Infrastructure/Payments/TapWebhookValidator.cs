using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Infrastructure.Payments.Configuration;

namespace Talaby.Infrastructure.Payments;

/// <summary>
/// Validates Tap payment webhook requests by computing HMAC-SHA256 of the
/// raw request body with the configured <see cref="TapOptions.SecretKey"/>
/// and comparing the result against the hashstring header received from Tap.
/// </summary>
public sealed class TapWebhookValidator(IOptions<TapOptions> options) : ITapWebhookValidator
{
    public bool IsValid(string rawPayload, string receivedHashstring)
    {
        if (string.IsNullOrWhiteSpace(rawPayload) || string.IsNullOrWhiteSpace(receivedHashstring))
            return false;

        var secretKey = options.Value.SecretKey;
        if (string.IsNullOrWhiteSpace(secretKey))
            return false;

        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var payloadBytes = Encoding.UTF8.GetBytes(rawPayload);

        var computedHashBytes = HMACSHA256.HashData(keyBytes, payloadBytes);
        var computedHashHex = Convert.ToHexString(computedHashBytes).ToLowerInvariant();
        var normalizedReceived = receivedHashstring.Trim().ToLowerInvariant();

        // Both strings must be the same length for FixedTimeEquals; if lengths differ
        // the signature is obviously invalid, but we still avoid short-circuit return.
        var computedBytes = Encoding.ASCII.GetBytes(computedHashHex);
        var receivedBytes = Encoding.ASCII.GetBytes(normalizedReceived);

        if (computedBytes.Length != receivedBytes.Length)
            return false;

        return CryptographicOperations.FixedTimeEquals(computedBytes, receivedBytes);
    }
}
