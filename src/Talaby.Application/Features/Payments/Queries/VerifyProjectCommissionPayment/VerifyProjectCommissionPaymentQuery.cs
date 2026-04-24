using MediatR;

namespace Talaby.Application.Features.Payments.Queries.VerifyProjectCommissionPayment;

public sealed record VerifyProjectCommissionPaymentQuery(Guid ProjectRequestId)
    : IRequest<VerifyProjectCommissionPaymentResponse>;
