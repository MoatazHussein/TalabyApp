using Talaby.Application.Features.Payments.Contracts;
using Talaby.Domain.Entities.Payments;

namespace Talaby.Application.Features.Payments.Services;

public interface ICommissionPaymentReconciler
{
    Task ApplyChargeOutcomeAsync(
        ProjectCommissionPayment commissionPayment,
        ProjectCommissionPaymentAttempt attempt,
        TapChargeOutcome outcome,
        string? failureMessage,
        CancellationToken cancellationToken);
}
