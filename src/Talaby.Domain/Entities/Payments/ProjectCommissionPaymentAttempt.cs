using Talaby.Domain.Enums;

namespace Talaby.Domain.Entities.Payments;

public class ProjectCommissionPaymentAttempt 
{
    private ProjectCommissionPaymentAttempt()
    {
    }

    private ProjectCommissionPaymentAttempt(
        Guid projectCommissionPaymentId,
        PaymentProvider providerName,
        string providerTransactionReference,
        string providerPaymentReference,
        string idempotencyKey,
        decimal requestedAmount,
        string requestedCurrency,
        DateTime createdAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerTransactionReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerPaymentReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedCurrency);

        if (requestedAmount <= 0)
            throw new ArgumentOutOfRangeException(nameof(requestedAmount));

        Id = Guid.NewGuid();
        ProjectCommissionPaymentId = projectCommissionPaymentId;
        ProviderName = providerName;
        ProviderTransactionReference = providerTransactionReference.Trim();
        ProviderPaymentReference = providerPaymentReference.Trim();
        IdempotencyKey = idempotencyKey.Trim();
        RequestedAmount = requestedAmount;
        RequestedCurrency = requestedCurrency.Trim().ToUpperInvariant();
        Status = ProjectCommissionPaymentAttemptStatus.Initiated;
        CreatedAtUtc = NormalizeUtc(createdAtUtc);
        UpdatedAtUtc = NormalizeUtc(createdAtUtc);
    }

    public Guid Id { get; private set; }

    public Guid ProjectCommissionPaymentId { get; private set; }

    public PaymentProvider ProviderName { get; private set; }

    public string? ProviderChargeId { get; private set; }

    public string ProviderTransactionReference { get; private set; } = default!;

    public string ProviderPaymentReference { get; private set; } = default!;

    public string IdempotencyKey { get; private set; } = default!;

    public decimal RequestedAmount { get; private set; }

    public string RequestedCurrency { get; private set; } = default!;

    public ProjectCommissionPaymentAttemptStatus Status { get; private set; }

    public string? CheckoutUrl { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public string? FailureReason { get; private set; }

    public ProjectCommissionPayment ProjectCommissionPayment { get; private set; } = default!;

    public static ProjectCommissionPaymentAttempt CreateInitiated(
        Guid projectCommissionPaymentId,
        PaymentProvider providerName,
        string providerTransactionReference,
        string providerPaymentReference,
        string idempotencyKey,
        decimal requestedAmount,
        string requestedCurrency,
        DateTime createdAtUtc)
    {
        return new ProjectCommissionPaymentAttempt(
            projectCommissionPaymentId,
            providerName,
            providerTransactionReference,
            providerPaymentReference,
            idempotencyKey,
            requestedAmount,
            requestedCurrency,
            createdAtUtc);
    }

    public void SetCheckoutUrlGenerated(
        string providerChargeId,
        string checkoutUrl,
        DateTime updatedAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerChargeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(checkoutUrl);

        ProviderChargeId = providerChargeId.Trim();
        CheckoutUrl = checkoutUrl.Trim();
        Status = ProjectCommissionPaymentAttemptStatus.CheckoutUrlGenerated;
        FailureReason = null;
        UpdatedAtUtc = NormalizeUtc(updatedAtUtc);
    }

    public void SetPaid(DateTime paidAtUtc)
    {
        Status = ProjectCommissionPaymentAttemptStatus.Paid;
        FailureReason = null;
        UpdatedAtUtc = NormalizeUtc(paidAtUtc);
    }

    public void SetFailed(string? failureReason, DateTime updatedAtUtc)
    {
        Status = ProjectCommissionPaymentAttemptStatus.Failed;
        FailureReason = failureReason?.Trim();
        UpdatedAtUtc = NormalizeUtc(updatedAtUtc);
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}

