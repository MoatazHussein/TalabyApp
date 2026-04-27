namespace Talaby.Application.Features.Dashboard.Queries.Client;

using Talaby.Domain.Enums;

public class ClientDashboardDto
{
    public ClientProjectStatsDto ProjectStats { get; set; } = default!;
    public ClientProposalStatsDto ProposalStats { get; set; } = default!;
    public ClientPaymentStatsDto PaymentStats { get; set; } = default!;
    public List<ClientRecentProjectDto> RecentProjects { get; set; } = [];
    public List<ClientCategoryActivityDto> TopCategories { get; set; } = [];
}

public class ClientProjectStatsDto
{
    public int Total { get; set; }
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int AwaitingCommissionPayment { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
}

public class ClientProposalStatsDto
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Accepted { get; set; }
    public int Rejected { get; set; }
    public int Cancelled { get; set; }
    public int Completed { get; set; }
}

public class ClientPaymentStatsDto
{
    public decimal TotalCommissionPaid { get; set; }
    public int PendingCount { get; set; }
    public int InitiatedCount { get; set; }
    public int PaidCount { get; set; }
    public int FailedCount { get; set; }
}

public class ClientRecentProjectDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public ProjectRequestStatus Status { get; set; }
    public int StoreCategoryId { get; set; }
    public string StoreCategoryName { get; set; } = default!;
    public int ProposalsCount { get; set; }
}

public class ClientCategoryActivityDto
{
    public int StoreCategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public int ProjectCount { get; set; }
}
