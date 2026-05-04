using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Payments.Queries.GetMyDueCommissionPayments;

public class DueCommissionPaymentDto
{
    public Guid ProjectRequestId { get; set; }
    public string ProjectTitle { get; set; } = default!;
    public ProjectRequestStatus ProjectStatus { get; set; }
    public ProjectCommissionPaymentStatus PaymentStatus { get; set; }
    public decimal CommissionAmount { get; set; }
    public string Currency { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
