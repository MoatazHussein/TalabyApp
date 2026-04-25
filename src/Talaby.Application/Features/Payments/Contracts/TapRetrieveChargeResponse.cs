namespace Talaby.Application.Features.Payments.Contracts;

public sealed record TapRetrieveChargeResponse(
    string ProviderChargeId,
    string ProviderStatus,
    string? FailureMessage);
