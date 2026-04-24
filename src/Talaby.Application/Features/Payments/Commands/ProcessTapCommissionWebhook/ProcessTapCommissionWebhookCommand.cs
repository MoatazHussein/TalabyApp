using MediatR;

namespace Talaby.Application.Features.Payments.Commands.ProcessTapCommissionWebhook;

public sealed record ProcessTapCommissionWebhookCommand(
    string RawPayload,
    string ReceivedHashstring) : IRequest;
