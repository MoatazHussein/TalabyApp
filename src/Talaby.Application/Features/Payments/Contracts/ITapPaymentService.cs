namespace Talaby.Application.Features.Payments.Contracts;
public interface ITapPaymentService
{
    Task<TapCreateChargeResponse> CreateChargeAsync(
        TapCreateChargeRequest request,
        CancellationToken cancellationToken = default);

    Task<TapRetrieveChargeResponse> RetrieveChargeAsync(
        string providerChargeId,
        CancellationToken cancellationToken = default);
}