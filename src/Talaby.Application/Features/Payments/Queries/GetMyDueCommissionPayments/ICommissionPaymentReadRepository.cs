namespace Talaby.Application.Features.Payments.Queries.GetMyDueCommissionPayments;

public interface ICommissionPaymentReadRepository
{
    Task<IReadOnlyList<DueCommissionPaymentDto>> GetDueCommissionPaymentsForClientAsync(
        Guid clientId,
        CancellationToken cancellationToken);
}
