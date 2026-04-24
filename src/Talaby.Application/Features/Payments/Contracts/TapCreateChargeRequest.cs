
namespace Talaby.Application.Features.Payments.Contracts;

public sealed record TapCreateChargeRequest(
    decimal Amount,
    string Currency,
    string Description,
    string ProviderTransactionReference,
    string ProviderPaymentReference,
    string IdempotencyKey,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhoneCountryCode,
    string CustomerPhoneNumber,
    string SourceId,
    string RedirectUrl,
    string PostUrl);