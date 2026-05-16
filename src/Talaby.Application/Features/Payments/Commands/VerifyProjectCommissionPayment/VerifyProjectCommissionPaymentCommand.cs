using MediatR;

namespace Talaby.Application.Features.Payments.Commands.VerifyProjectCommissionPayment;

public sealed record VerifyProjectCommissionPaymentCommand(Guid ProjectRequestId)
    : IRequest<VerifyProjectCommissionPaymentResponse>;
