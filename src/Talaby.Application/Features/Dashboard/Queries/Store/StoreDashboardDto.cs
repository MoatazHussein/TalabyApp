namespace Talaby.Application.Features.Dashboard.Queries.Store;

using Talaby.Domain.Enums;

public class StoreDashboardDto
{
    public StoreProposalStatsDto ProposalStats { get; set; } = default!;
    public StoreWorkStatsDto WorkStats { get; set; } = default!;
    public StoreEarningsStatsDto EarningsStats { get; set; } = default!;
    public StoreCommissionStatsDto CommissionStats { get; set; } = default!;
    public List<StoreRecentProposalDto> RecentProposals { get; set; } = [];
}

public class StoreProposalStatsDto
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Accepted { get; set; }
    public int Rejected { get; set; }
    public int Cancelled { get; set; }
    public int Completed { get; set; }
}

public class StoreWorkStatsDto
{
    public int Active { get; set; }
    public int AwaitingCommissionPayment { get; set; }
    public int Completed { get; set; }
}

public class StoreEarningsStatsDto
{
    public decimal TotalAcceptedProposalAmount { get; set; }
    public decimal TotalCompletedProposalAmount { get; set; }
    public decimal AverageAcceptedProposalAmount { get; set; }
}

public class StoreCommissionStatsDto
{
    public decimal TotalCommissionPaid { get; set; }
    public int PendingCount { get; set; }
    public int InitiatedCount { get; set; }
    public int PaidCount { get; set; }
    public int FailedCount { get; set; }
}

public class StoreRecentProposalDto
{
    public Guid Id { get; set; }
    public Guid ProjectRequestId { get; set; }
    public string ProjectTitle { get; set; } = default!;
    public decimal ProposedAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public ProjectProposalStatus ProposalStatus { get; set; }
    public ProjectRequestStatus ProjectStatus { get; set; }
}
