using Microsoft.EntityFrameworkCore;
using Talaby.Application.Features.Payments.Queries.GetMyDueCommissionPayments;
using Talaby.Domain.Enums;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Payments;

public class CommissionPaymentReadRepository(TalabyDbContext context) : ICommissionPaymentReadRepository
{
    public async Task<IReadOnlyList<DueCommissionPaymentDto>> GetDueCommissionPaymentsForClientAsync(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        return await context.ProjectCommissionPayments
            .AsNoTracking()
            .Where(payment => payment.ProjectRequest.CreatorId == clientId)
            .Where(payment => payment.ProjectRequest.Status == ProjectRequestStatus.AwaitingCommissionPayment)
            .Where(payment => payment.Status != ProjectCommissionPaymentStatus.Paid)
            .OrderByDescending(payment => payment.CreatedAtUtc)
            .ThenByDescending(payment => payment.ProjectRequestId)
            .Select(payment => new DueCommissionPaymentDto
            {
                ProjectRequestId = payment.ProjectRequestId,
                ProjectTitle = payment.ProjectRequest.Title,
                ProjectStatus = payment.ProjectRequest.Status,
                PaymentStatus = payment.Status,
                CommissionAmount = payment.CommissionAmount,
                Currency = payment.Currency,
                CreatedAt = payment.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
