using Microsoft.Extensions.Logging;
using Talaby.Application.Features.Payments.Contracts;
using Talaby.Domain.Entities.Payments;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Payments.Services;

public sealed class CommissionPaymentReconciler(
    IProjectRequestRepository projectRequestRepository,
    ILogger<CommissionPaymentReconciler> logger)
    : ICommissionPaymentReconciler
{
    public async Task ApplyChargeOutcomeAsync(
        ProjectCommissionPayment commissionPayment,
        ProjectCommissionPaymentAttempt attempt,
        TapChargeOutcome outcome,
        string? failureMessage,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        switch (outcome)
        {
            case TapChargeOutcome.Success:
                attempt.SetPaid(now);
                commissionPayment.MarkPaid(now);

                var projectRequest = await projectRequestRepository
                    .GetByIdAsync(commissionPayment.ProjectRequestId);

                if (projectRequest is null)
                {
                    logger.LogError(
                        "CommissionPaymentReconciler: ProjectRequest not found after charge confirmed CAPTURED. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}",
                        commissionPayment.ProjectRequestId, commissionPayment.Id);
                    return;
                }

                projectRequest.MarkCompleted();

                logger.LogInformation(
                    "CommissionPaymentReconciler: payment confirmed. ProjectRequestId={ProjectRequestId}, CommissionPaymentId={CommissionPaymentId}, AttemptId={AttemptId}, ChargeId={ChargeId}",
                    projectRequest.Id, commissionPayment.Id, attempt.Id, attempt.ProviderChargeId);
                break;

            case TapChargeOutcome.TerminalFailure:
                attempt.SetFailed(failureMessage, now);
                commissionPayment.MarkFailed();

                logger.LogWarning(
                    "CommissionPaymentReconciler: payment failed. CommissionPaymentId={CommissionPaymentId}, AttemptId={AttemptId}, ChargeId={ChargeId}, Reason={Reason}",
                    commissionPayment.Id, attempt.Id, attempt.ProviderChargeId, failureMessage);
                break;

            default:
                logger.LogDebug(
                    "CommissionPaymentReconciler: charge still non-final. CommissionPaymentId={CommissionPaymentId}, ChargeId={ChargeId}",
                    commissionPayment.Id, attempt.ProviderChargeId);
                return;
        }
    }
}
