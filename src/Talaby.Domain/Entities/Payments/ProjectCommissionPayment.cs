using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Enums;

namespace Talaby.Domain.Entities.Payments;

public class ProjectCommissionPayment
{
    private readonly List<ProjectCommissionPaymentAttempt> _paymentAttempts = new();

    private ProjectCommissionPayment()
    {
    }

    private ProjectCommissionPayment(
        Guid projectRequestId,
        Guid projectProposalId,
        decimal proposalAmountSnapshot,
        decimal commissionPercentage,
        string currency,
        DateTime createdAtUtc)
    {
        if (proposalAmountSnapshot <= 0)
            throw new ArgumentOutOfRangeException(nameof(proposalAmountSnapshot));

        if (commissionPercentage <= 0)
            throw new ArgumentOutOfRangeException(nameof(commissionPercentage));

        ArgumentException.ThrowIfNullOrWhiteSpace(currency);

        Id = Guid.NewGuid();
        ProjectRequestId = projectRequestId;
        ProjectProposalId = projectProposalId;
        ProposalAmountSnapshot = proposalAmountSnapshot;
        CommissionPercentage = commissionPercentage;
        CommissionAmount = CalculateCommissionAmount(proposalAmountSnapshot, commissionPercentage);
        Currency = currency.Trim().ToUpperInvariant();
        Status = ProjectCommissionPaymentStatus.Pending;
        CreatedAtUtc = NormalizeUtc(createdAtUtc);
    }

    public Guid Id { get; private set; }

    public Guid ProjectRequestId { get; private set; }

    public Guid ProjectProposalId { get; private set; }

    public decimal ProposalAmountSnapshot { get; private set; }

    public decimal CommissionPercentage { get; private set; }

    public decimal CommissionAmount { get; private set; }

    public string Currency { get; private set; } = default!;

    public ProjectCommissionPaymentStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? PaidAtUtc { get; private set; }

    public ProjectRequest ProjectRequest { get; private set; } = default!;

    public ProjectProposal ProjectProposal { get; private set; } = default!;

    public IReadOnlyCollection<ProjectCommissionPaymentAttempt> PaymentAttempts => _paymentAttempts.AsReadOnly();

    public static ProjectCommissionPayment Create(
        Guid projectRequestId,
        Guid projectProposalId,
        decimal proposalAmountSnapshot,
        decimal commissionPercentage,
        string currency,
        DateTime createdAtUtc)
    {
        return new ProjectCommissionPayment(
            projectRequestId,
            projectProposalId,
            proposalAmountSnapshot,
            commissionPercentage,
            currency,
            createdAtUtc);
    }

    public void MarkInitiated()
    {
        if (Status == ProjectCommissionPaymentStatus.Paid)
            throw new InvalidOperationException("Paid payment cannot move back to initiated.");

        Status = ProjectCommissionPaymentStatus.Initiated;
    }

    public void MarkFailed()
    {
        if (Status == ProjectCommissionPaymentStatus.Paid)
            throw new InvalidOperationException("Paid payment cannot move to failed.");

        Status = ProjectCommissionPaymentStatus.Failed;
    }

    public void MarkPaid(DateTime paidAtUtc)
    {
        Status = ProjectCommissionPaymentStatus.Paid;
        PaidAtUtc = NormalizeUtc(paidAtUtc);
    }

    public void AddAttempt(ProjectCommissionPaymentAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        _paymentAttempts.Add(attempt);
    }

    private static decimal CalculateCommissionAmount(decimal proposalAmount, decimal percentage)
    {
        return Math.Round(proposalAmount * (percentage / 100m), 3, MidpointRounding.AwayFromZero);
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