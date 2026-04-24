using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Domain.Exceptions;
using Talaby.Infrastructure.Payments.Configuration;

namespace Talaby.Infrastructure.Payments;

public class TapPaymentService(
    IHttpClientFactory httpClientFactory,
    IOptions<TapOptions> options,
    ILogger<TapPaymentService> logger)
    : ITapPaymentService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<TapCreateChargeResponse> CreateChargeAsync(
        TapCreateChargeRequest request,
        CancellationToken cancellationToken = default)
    {
        var opts = options.Value;

        var payload = BuildPayload(request, opts);
        var json = JsonSerializer.Serialize(payload, JsonOptions);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v2/charges")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        httpRequest.Headers.Add("Idempotency-Key", request.IdempotencyKey);

        logger.LogDebug(
            "Calling Tap create-charge. IdempotencyKey={Key}, Amount={Amount} {Currency}",
            request.IdempotencyKey, request.Amount, request.Currency);

        var httpClient = httpClientFactory.CreateClient("TapClient");
        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = ExtractTapError(responseBody);
            logger.LogWarning(
                "Tap create-charge failed. Status={Status}, Error={Error}",
                (int)response.StatusCode, errorMessage);

            throw new PaymentGatewayException(
                $"Tap create-charge failed ({(int)response.StatusCode}): {errorMessage}");
        }

        var tapResponse = JsonSerializer.Deserialize<TapChargeResponse>(responseBody, JsonOptions);

        if (tapResponse is null || string.IsNullOrWhiteSpace(tapResponse.Id))
        {
            logger.LogError("Tap returned success but response was invalid. Body={Body}", responseBody);
            throw new PaymentGatewayException("Tap returned an unexpected response format.");
        }

        var checkoutUrl = tapResponse.Transaction?.Url;
        if (string.IsNullOrWhiteSpace(checkoutUrl))
        {
            logger.LogError("Tap response missing transaction.url. Body={Body}", responseBody);
            throw new PaymentGatewayException("Tap did not return a checkout URL.");
        }

        logger.LogInformation(
            "Tap charge created. ChargeId={ChargeId}, Status={Status}",
            tapResponse.Id, tapResponse.Status);

        return new TapCreateChargeResponse(
            ProviderChargeId: tapResponse.Id,
            CheckoutUrl: checkoutUrl,
            ProviderStatus: tapResponse.Status ?? string.Empty);
    }

    // ── Payload builder ─────────────────────────────────────────────────────

    private static TapChargePayload BuildPayload(TapCreateChargeRequest request, TapOptions opts)
    {
        return new TapChargePayload
        {
            Amount = request.Amount,
            Currency = request.Currency,
            CustomerInitiated = true,
            ThreeDSecure = true,
            SaveCard = false,
            Description = request.Description,
            Reference = new TapReference
            {
                Transaction = request.ProviderTransactionReference,
                Order = request.ProviderPaymentReference
            },
            Customer = new TapCustomer
            {
                FirstName = request.CustomerName,
                Email = request.CustomerEmail,
                Phone = new TapPhone
                {
                    CountryCode = request.CustomerPhoneCountryCode,
                    Number = request.CustomerPhoneNumber
                }
            },
            Merchant = string.IsNullOrWhiteSpace(opts.MerchantId)
                ? null
                : new TapMerchant { Id = opts.MerchantId },
            Source = new TapSource { Id = request.SourceId },
            Post = new TapCallbackUrl { Url = request.PostUrl },
            Redirect = new TapCallbackUrl { Url = request.RedirectUrl }
        };
    }

    // ── Error extraction ─────────────────────────────────────────────────────

    private static string ExtractTapError(string responseBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.TryGetProperty("errors", out var errors) && errors.GetArrayLength() > 0)
            {
                var first = errors[0];
                var code = first.TryGetProperty("code", out var c) ? c.GetString() : null;
                var desc = first.TryGetProperty("description", out var d) ? d.GetString() : null;
                return $"[{code}] {desc}".Trim();
            }
            if (doc.RootElement.TryGetProperty("message", out var msg))
                return msg.GetString() ?? responseBody;
        }
        catch
        {
            // If JSON parsing fails, fall through to raw body
        }

        return responseBody.Length > 400 ? responseBody[..400] : responseBody;
    }

    // ── Private Tap API JSON models ──────────────────────────────────────────

    private sealed class TapChargePayload
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; } = default!;

        [JsonPropertyName("customer_initiated")]
        public bool CustomerInitiated { get; init; }

        [JsonPropertyName("threeDSecure")]
        public bool ThreeDSecure { get; init; }

        [JsonPropertyName("save_card")]
        public bool SaveCard { get; init; }

        [JsonPropertyName("description")]
        public string Description { get; init; } = default!;

        [JsonPropertyName("reference")]
        public TapReference Reference { get; init; } = default!;

        [JsonPropertyName("customer")]
        public TapCustomer Customer { get; init; } = default!;

        [JsonPropertyName("merchant")]
        public TapMerchant? Merchant { get; init; }

        [JsonPropertyName("source")]
        public TapSource Source { get; init; } = default!;

        [JsonPropertyName("post")]
        public TapCallbackUrl Post { get; init; } = default!;

        [JsonPropertyName("redirect")]
        public TapCallbackUrl Redirect { get; init; } = default!;
    }

    private sealed class TapReference
    {
        [JsonPropertyName("transaction")]
        public string Transaction { get; init; } = default!;

        [JsonPropertyName("order")]
        public string Order { get; init; } = default!;
    }

    private sealed class TapCustomer
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; init; } = default!;

        [JsonPropertyName("email")]
        public string Email { get; init; } = default!;

        [JsonPropertyName("phone")]
        public TapPhone Phone { get; init; } = default!;
    }

    private sealed class TapPhone
    {
        [JsonPropertyName("country_code")]
        public string CountryCode { get; init; } = default!;

        [JsonPropertyName("number")]
        public string Number { get; init; } = default!;
    }

    private sealed class TapMerchant
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = default!;
    }

    private sealed class TapSource
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = default!;
    }

    private sealed class TapCallbackUrl
    {
        [JsonPropertyName("url")]
        public string Url { get; init; } = default!;
    }

    // ── Tap API response models ──────────────────────────────────────────────

    private sealed class TapChargeResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; init; }

        [JsonPropertyName("status")]
        public string? Status { get; init; }

        [JsonPropertyName("transaction")]
        public TapTransactionDetails? Transaction { get; init; }
    }

    private sealed class TapTransactionDetails
    {
        [JsonPropertyName("url")]
        public string? Url { get; init; }
    }
}
