using MediatR;

namespace Talaby.Application.Features.Payments.Commands.SyncTapPaymentStatus;

public sealed record SyncTapPaymentStatusCommand(Guid ProjectRequestId) : IRequest;
