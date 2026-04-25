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
        logger.LogDebug(
            "SyncTapPaymentStatus started. ProjectRequestId={ProjectRequestId}",
            request.ProjectRequestId);

        var commissionPayment = await commissionPaymentRepository
            .GetWithAttemptsByProjectRequestIdAsync(request.ProjectRequestId, cancellationToken);

        if (commissionPayment is null)
        {
            logger.LogDebug(
                "SyncTapPaymentStatus: no commission payment found. ProjectRequestId={ProjectRequestId}",
                request.ProjectRequestId);
            return;
        }

        // Already in a final state — nothing to poll.
        if (commissionPayment.IsFinalState())
        {
            logger.LogDebug(
                "SyncTapPaymentStatus: skipped, payment already in final state. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}, PaymentStatus={PaymentStatus}",
                request.ProjectRequestId, commissionPayment.Id, commissionPayment.Status);
            return;
        }

        // Find the latest attempt that has been submitted to Tap.
        var latestAttempt = commissionPayment.PaymentAttempts
            .Where(a => !string.IsNullOrWhiteSpace(a.ProviderChargeId))
            .MaxBy(a => a.CreatedAtUtc);

        if (latestAttempt is null)
        {
            logger.LogDebug(
                "SyncTapPaymentStatus: no submitted attempt found. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}",
                request.ProjectRequestId, commissionPayment.Id);
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
                "SyncTapPaymentStatus: Tap retrieve-charge failed. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}, ChargeId={ChargeId}. Local state unchanged.",
                request.ProjectRequestId, commissionPayment.Id, latestAttempt.ProviderChargeId);
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
                        "SyncTapPaymentStatus: ProjectRequest not found after charge confirmed CAPTURED. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}",
                        commissionPayment.ProjectRequestId, commissionPayment.Id);
                    return;
                }

                projectRequest.MarkCompleted();

                logger.LogInformation(
                    "SyncTapPaymentStatus: payment confirmed via retrieve-charge. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}, AttemptId={AttemptId}, ChargeId={ChargeId}",
                    request.ProjectRequestId, commissionPayment.Id, latestAttempt.Id, latestAttempt.ProviderChargeId);
                break;

            case TapChargeOutcome.TerminalFailure:
                latestAttempt.SetFailed(tapCharge.FailureMessage, now);
                commissionPayment.MarkFailed();

                logger.LogWarning(
                    "SyncTapPaymentStatus: payment failed via retrieve-charge. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}, AttemptId={AttemptId}, ChargeId={ChargeId}, ProviderStatus={ProviderStatus}",
                    request.ProjectRequestId, commissionPayment.Id, latestAttempt.Id, latestAttempt.ProviderChargeId, tapCharge.ProviderStatus);
                break;

            default:
                logger.LogDebug(
                    "SyncTapPaymentStatus: charge still non-final. ProjectRequestId={ProjectRequestId}, ChargeId={ChargeId}, ProviderStatus={ProviderStatus}",
                    request.ProjectRequestId, latestAttempt.ProviderChargeId, tapCharge.ProviderStatus);
                return; 
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
