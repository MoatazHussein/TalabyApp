namespace Talaby.Application.Features.Payments.Contracts;
public interface ITapPaymentService
{
    Task<TapCreateChargeResponse> CreateChargeAsync(
        TapCreateChargeRequest request,
        CancellationToken cancellationToken = default);
}