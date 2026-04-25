using MediatR;
using Microsoft.Extensions.Logging;
using Talaby.Application.Common.Interfaces;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Domain.Repositories.Payments;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Payments.Commands.SyncTapPaymentStatus;

public sealed class SyncTapPaymentStatusCommandHandler(
    IProjectCommissionPaymentRepository commissionPaymentRepository,
    IProjectRequestRepository projectRequestRepository,
    ITapPaymentService tapPaymentService,
    IUnitOfWork unitOfWork,
    ILogger<SyncTapPaymentStatusCommandHandler> logger)
    : IRequestHandler<SyncTapPaymentStatusCommand>
{
    public async Task Handle(SyncTapPaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var commissionPayment = await commissionPaymentRepository
            .GetWithAttemptsByProjectRequestIdAsync(request.ProjectRequestId, cancellationToken);

        if (commissionPayment is null)
        {
            logger.LogDebug(
                "SyncTapPaymentStatus: no commission payment found for ProjectRequest {Id}. Nothing to sync.",
                request.ProjectRequestId);
            return;
        }

        // Already in a final state — nothing to poll.
        if (commissionPayment.IsFinalState())
        {
            logger.LogDebug(
                "SyncTapPaymentStatus: payment {PaymentId} is already in a final state. Skipping Tap call.",
                commissionPayment.Id);
            return;
        }

        // Find the latest attempt that has been submitted to Tap.
        var latestAttempt = commissionPayment.PaymentAttempts
            .Where(a => !string.IsNullOrWhiteSpace(a.ProviderChargeId))
            .MaxBy(a => a.CreatedAtUtc);

        if (latestAttempt is null)
        {
            logger.LogDebug(
                "SyncTapPaymentStatus: no submitted attempt found for payment {PaymentId}. Nothing to sync.",
                commissionPayment.Id);
            return;
        }

        TapRetrieveChargeResponse tapCharge;
        try
        {
            tapCharge = await tapPaymentService.RetrieveChargeAsync(
                latestAttempt.ProviderChargeId!, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "SyncTapPaymentStatus: Tap retrieve-charge failed for ChargeId={ChargeId}. Local state unchanged.",
                latestAttempt.ProviderChargeId);
            return;
        }

        var outcome = TapChargeStatusMapper.Map(tapCharge.ProviderStatus);
        var now = DateTime.UtcNow;

        switch (outcome)
        {
            case TapChargeOutcome.Success:
                latestAttempt.SetPaid(now);
                commissionPayment.MarkPaid(now);

                var projectRequest = await projectRequestRepository
                    .GetByIdAsync(commissionPayment.ProjectRequestId);

                if (projectRequest is null)
                {
                    logger.LogError(
                        "SyncTapPaymentStatus: ProjectRequest {Id} not found after charge confirmed CAPTURED.",
                        commissionPayment.ProjectRequestId);
                    return;
                }

                projectRequest.MarkCompleted();

                logger.LogInformation(
                    "SyncTapPaymentStatus: payment confirmed. PaymentId={PaymentId}, ChargeId={ChargeId}",
                    commissionPayment.Id, latestAttempt.ProviderChargeId);
                break;

            case TapChargeOutcome.TerminalFailure:
                latestAttempt.SetFailed(tapCharge.FailureMessage, now);
                commissionPayment.MarkFailed();

                logger.LogWarning(
                    "SyncTapPaymentStatus: payment failed. PaymentId={PaymentId}, ChargeId={ChargeId}, Status={Status}",
                    commissionPayment.Id, latestAttempt.ProviderChargeId, tapCharge.ProviderStatus);
                break;

            default:
                logger.LogDebug(
                    "SyncTapPaymentStatus: charge {ChargeId} still non-final ({Status}). No state change.",
                    latestAttempt.ProviderChargeId, tapCharge.ProviderStatus);
                return; 
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
