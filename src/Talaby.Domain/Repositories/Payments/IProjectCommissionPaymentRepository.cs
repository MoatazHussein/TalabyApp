using Talaby.Domain.Entities.Payments;

namespace Talaby.Domain.Repositories.Payments;

public interface IProjectCommissionPaymentRepository
{
    Task AddAsync(ProjectCommissionPayment payment, CancellationToken cancellationToken = default);

    Task<bool> ExistsForProjectAsync(Guid projectRequestId, CancellationToken cancellationToken = default);

    Task<ProjectCommissionPayment?> GetByProjectRequestIdAsync(Guid projectRequestId, CancellationToken cancellationToken = default);

    Task<ProjectCommissionPayment?> GetWithAttemptsByProjectRequestIdAsync(Guid projectRequestId, CancellationToken cancellationToken = default);

    Task AddAttemptAsync(ProjectCommissionPaymentAttempt attempt, CancellationToken cancellationToken = default);

    Task<ProjectCommissionPayment?> GetWithAttemptsByProviderChargeIdAsync(
        string providerChargeId, CancellationToken cancellationToken = default);
}
