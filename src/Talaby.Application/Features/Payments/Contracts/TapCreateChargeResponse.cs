
namespace Talaby.Application.Features.Payments.Contracts;

public sealed record TapCreateChargeResponse(
    string ProviderChargeId,
    string CheckoutUrl,
    string ProviderStatus);