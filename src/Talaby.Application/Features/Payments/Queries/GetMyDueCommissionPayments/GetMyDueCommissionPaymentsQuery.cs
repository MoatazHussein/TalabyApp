using MediatR;

namespace Talaby.Application.Features.Payments.Queries.GetMyDueCommissionPayments;

public record GetMyDueCommissionPaymentsQuery
    : IRequest<IReadOnlyList<DueCommissionPaymentDto>>;
