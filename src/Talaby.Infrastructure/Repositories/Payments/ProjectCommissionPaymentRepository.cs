using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Entities.Payments;
using Talaby.Domain.Repositories.Payments;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Payments;

public class ProjectCommissionPaymentRepository(TalabyDbContext dbContext)
    : IProjectCommissionPaymentRepository
{
    public async Task AddAsync(ProjectCommissionPayment payment, CancellationToken cancellationToken = default)
    {
        await dbContext.ProjectCommissionPayments.AddAsync(payment, cancellationToken);
    }

    public Task<bool> ExistsForProjectAsync(Guid projectRequestId, CancellationToken cancellationToken = default)
    {
        return dbContext.ProjectCommissionPayments
            .AnyAsync(p => p.ProjectRequestId == projectRequestId, cancellationToken);
    }

    public Task<ProjectCommissionPayment?> GetByProjectRequestIdAsync(
        Guid projectRequestId, CancellationToken cancellationToken = default)
    {
        return dbContext.ProjectCommissionPayments
            .FirstOrDefaultAsync(p => p.ProjectRequestId == projectRequestId, cancellationToken);
    }

    public async Task AddAttemptAsync(
        ProjectCommissionPaymentAttempt attempt, CancellationToken cancellationToken = default)
    {
        await dbContext.ProjectCommissionPaymentAttempts.AddAsync(attempt, cancellationToken);
    }

    public async Task<ProjectCommissionPayment?> GetWithAttemptsByProviderChargeIdAsync(
        string providerChargeId, CancellationToken cancellationToken = default)
    {
        // Two-step: locate the parent payment ID via the attempt, then load the full
        // aggregate with attempts so EF change tracking covers everything we mutate.
        var paymentId = await dbContext.ProjectCommissionPaymentAttempts
            .Where(a => a.ProviderChargeId == providerChargeId)
            .Select(a => (Guid?)a.ProjectCommissionPaymentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (paymentId is null) return null;

        return await dbContext.ProjectCommissionPayments
            .Include(p => p.PaymentAttempts)
            .FirstOrDefaultAsync(p => p.Id == paymentId.Value, cancellationToken);
    }
}
