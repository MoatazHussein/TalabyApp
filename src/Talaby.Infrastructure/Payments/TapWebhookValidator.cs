using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Infrastructure.Payments.Configuration;

namespace Talaby.Infrastructure.Payments;

/// <summary>
/// Validates Tap payment webhook requests using Tap's hashstring algorithm:
/// HMAC-SHA256 of a concatenated string of specific charge fields (NOT the raw body).
/// </summary>
public sealed class TapWebhookValidator(IOptions<TapOptions> options, ILogger<TapWebhookValidator> logger) : ITapWebhookValidator
{
    public bool IsValid(string rawPayload, string receivedHashstring)
    {
        if (string.IsNullOrWhiteSpace(rawPayload))
        {
            logger.LogWarning("Tap webhook validation failed: raw payload is null or empty.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(receivedHashstring))
        {
            logger.LogWarning("Tap webhook validation failed: hashstring header is missing or empty.");
            return false;
        }

        var secretKey = options.Value.SecretKey;
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            logger.LogError("Tap webhook validation failed: Tap:SecretKey is not configured.");
            return false;
        }

        string toBeHashed;
        try
        {
            toBeHashed = BuildHashInput(rawPayload);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Tap webhook validation failed: could not extract hash fields from payload.");
            return false;
        }

        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var inputBytes = Encoding.UTF8.GetBytes(toBeHashed);

        var computedHashBytes = HMACSHA256.HashData(keyBytes, inputBytes);
        var computedHashHex = Convert.ToHexString(computedHashBytes).ToLowerInvariant();
        var normalizedReceived = receivedHashstring.Trim().ToLowerInvariant();

        var computedBytes = Encoding.ASCII.GetBytes(computedHashHex);
        var receivedBytes = Encoding.ASCII.GetBytes(normalizedReceived);

        if (computedBytes.Length != receivedBytes.Length)
        {
            logger.LogWarning(
                "Tap webhook validation failed: hash length mismatch. " +
                "ComputedLength={ComputedLength}, ReceivedLength={ReceivedLength}, " +
                "HashInput={HashInput}, Received={ReceivedHash}",
                computedBytes.Length, receivedBytes.Length, toBeHashed, normalizedReceived);
            return false;
        }

        var isValid = CryptographicOperations.FixedTimeEquals(computedBytes, receivedBytes);

        if (!isValid)
        {
            logger.LogWarning(
                "Tap webhook validation failed: HMAC mismatch. " +
                "HashInput={HashInput}, Computed={ComputedHash}, Received={ReceivedHash}",
                toBeHashed, computedHashHex, normalizedReceived);
            return false;
        }

        logger.LogDebug(
            "Tap webhook signature validated successfully. PayloadLength={PayloadLength}",
            rawPayload.Length);

        return true;
    }

    private static string BuildHashInput(string rawPayload)
    {
        using var doc = JsonDocument.Parse(rawPayload);
        var root = doc.RootElement;

        var id = GetString(root, "id");
        var amount = GetRawNumber(root, "amount");
        var currency = GetString(root, "currency");
        var status = GetString(root, "status");

        // reference.gateway — pass empty string if not available (per Tap docs)
        var gatewayReference = string.Empty;
        var paymentReference = string.Empty;
        if (root.TryGetProperty("reference", out var refEl))
        {
            gatewayReference = refEl.TryGetProperty("gateway", out var gwEl)
                ? gwEl.GetString() ?? string.Empty
                : string.Empty;

            paymentReference = refEl.TryGetProperty("payment", out var pmEl)
                ? pmEl.GetString() ?? string.Empty
                : string.Empty;
        }

        // transaction.created
        var created = string.Empty;
        if (root.TryGetProperty("transaction", out var txEl))
        {
            created = txEl.TryGetProperty("created", out var createdEl)
                ? createdEl.GetRawText().Trim('"')
                : string.Empty;
        }

        return $"x_id{id}x_amount{amount}x_currency{currency}" +
               $"x_gateway_reference{gatewayReference}" +
               $"x_payment_reference{paymentReference}" +
               $"x_status{status}x_created{created}";
    }

    private static string GetString(JsonElement el, string property)
        => el.TryGetProperty(property, out var p) ? p.GetString() ?? string.Empty : string.Empty;

    private static string GetRawNumber(JsonElement el, string property)
        => el.TryGetProperty(property, out var p) ? p.GetRawText() : string.Empty;
}

