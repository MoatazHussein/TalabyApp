using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Payments.Queries.VerifyProjectCommissionPayment;

public sealed record VerifyProjectCommissionPaymentResponse(
    Guid ProjectRequestId,
    ProjectRequestStatus ProjectStatus,
    ProjectCommissionPaymentStatus? PaymentStatus,
    bool IsPaid,
    DateTime? PaidAt);
