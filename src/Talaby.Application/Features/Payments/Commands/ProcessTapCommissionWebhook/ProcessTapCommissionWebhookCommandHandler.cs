using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Application.Common.Interfaces;
using Talaby.Domain.Enums;
using Talaby.Domain.Exceptions;
using Talaby.Domain.Repositories.Payments;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Payments.Commands.ProcessTapCommissionWebhook;

public sealed class ProcessTapCommissionWebhookCommandHandler(
    ITapWebhookValidator webhookValidator,
    IProjectCommissionPaymentRepository commissionPaymentRepository,
    IProjectRequestRepository projectRequestRepository,
    IUnitOfWork unitOfWork,
    ILogger<ProcessTapCommissionWebhookCommandHandler> logger)
    : IRequestHandler<ProcessTapCommissionWebhookCommand>
{
    // Tap's terminal success status for a charge.
    private const string TapCapturedStatus = "CAPTURED";

    // Terminal failure statuses that should mark the attempt/payment as failed.
    private static readonly HashSet<string> TerminalFailureStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "DECLINED", "FAILED", "CANCELLED", "ABANDONED", "RESTRICTED", "TIMEDOUT", "VOID"
    };

    public async Task Handle(
        ProcessTapCommissionWebhookCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Authenticate — reject anything with an invalid signature.
        if (!webhookValidator.IsValid(request.RawPayload, request.ReceivedHashstring))
        {
            logger.LogWarning("Tap webhook rejected: invalid hashstring.");
            throw new UnauthorizedAccessException("Invalid Tap webhook signature.");
        }

        // 2. Parse minimal fields we need from the charge payload.
        string chargeId;
        string chargeStatus;
        string? failureMessage;

        try
        {
            using var doc = JsonDocument.Parse(request.RawPayload);
            var root = doc.RootElement;

            chargeId = root.GetProperty("id").GetString()
                ?? throw new PaymentGatewayException("Tap webhook payload missing 'id'.");

            chargeStatus = root.GetProperty("status").GetString()
                ?? throw new PaymentGatewayException("Tap webhook payload missing 'status'.");

            failureMessage = root.TryGetProperty("response", out var resp)
                && resp.TryGetProperty("message", out var msg)
                    ? msg.GetString()
                    : null;
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Tap webhook payload is missing required fields.");
            throw new PaymentGatewayException("Tap webhook payload is missing required fields.");
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Tap webhook payload could not be parsed as JSON.");
            throw new PaymentGatewayException("Tap webhook payload is not valid JSON.");
        }

        logger.LogInformation(
            "Processing Tap webhook. ChargeId={ChargeId}, Status={Status}",
            chargeId, chargeStatus);

        // 3. Load the commission payment that owns this charge (includes all attempts).
        var commissionPayment = await commissionPaymentRepository
            .GetWithAttemptsByProviderChargeIdAsync(chargeId, cancellationToken);

        if (commissionPayment is null)
        {
            // Tap may send webhooks for test charges or other untracked charges — log and ignore.
            logger.LogWarning(
                "Tap webhook received for unknown ChargeId={ChargeId}. No matching payment found.",
                chargeId);
            return;
        }

        // 4. Idempotency — if the payment is already confirmed paid, this is a duplicate webhook.
        if (commissionPayment.Status == ProjectCommissionPaymentStatus.Paid)
        {
            logger.LogInformation(
                "Tap webhook is a duplicate for already-paid ChargeId={ChargeId}. No-op.",
                chargeId);
            return;
        }

        // 5. Locate the specific attempt for this charge.
        var attempt = commissionPayment.PaymentAttempts
            .FirstOrDefault(a => string.Equals(a.ProviderChargeId, chargeId, StringComparison.OrdinalIgnoreCase));

        if (attempt is null)
        {
            // Should not happen given GetWithAttemptsByProviderChargeIdAsync, but guard anyway.
            logger.LogError(
                "Tap webhook: attempt for ChargeId={ChargeId} not found inside loaded payment {PaymentId}.",
                chargeId, commissionPayment.Id);
            return;
        }

        var now = DateTime.UtcNow;

        if (string.Equals(chargeStatus, TapCapturedStatus, StringComparison.OrdinalIgnoreCase))
        {
            // 6a. Payment succeeded — mark attempt, payment, and project request as completed.
            attempt.SetPaid(now);
            commissionPayment.MarkPaid(now);

            var projectRequest = await projectRequestRepository
                .GetByIdAsync(commissionPayment.ProjectRequestId);

            if (projectRequest is null)
            {
                logger.LogError(
                    "Tap webhook: ProjectRequest {ProjectRequestId} not found for ChargeId={ChargeId}.",
                    commissionPayment.ProjectRequestId, chargeId);
                throw new InvalidOperationException(
                    $"ProjectRequest {commissionPayment.ProjectRequestId} not found.");
            }

            projectRequest.MarkCompleted();

            logger.LogInformation(
                "Commission payment confirmed. PaymentId={PaymentId}, ProjectRequestId={ProjectRequestId}",
                commissionPayment.Id, projectRequest.Id);
        }
        else if (TerminalFailureStatuses.Contains(chargeStatus))
        {
            // 6b. Payment failed — mark attempt and payment accordingly.
            attempt.SetFailed(failureMessage, now);
            commissionPayment.MarkFailed();

            logger.LogWarning(
                "Commission payment failed. PaymentId={PaymentId}, TapStatus={Status}, Reason={Reason}",
                commissionPayment.Id, chargeStatus, failureMessage);
        }
        else
        {
            // Non-terminal status (e.g., INITIATED, INPROGRESS) — nothing to do yet.
            logger.LogInformation(
                "Tap webhook received non-terminal status {Status} for ChargeId={ChargeId}. No action taken.",
                chargeStatus, chargeId);
            return;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
