using MediatR;

namespace Talaby.Application.Features.Payments.Commands.CreateProjectCommissionCheckout;

public record CreateProjectCommissionCheckoutCommand(Guid ProjectRequestId)
    : IRequest<CreateProjectCommissionCheckoutResponse>;
