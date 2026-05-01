namespace Talaby.Application.Features.Dashboard.Queries.Admin;

public class AdminDashboardDto
{
    public UserStatsDto UserStats { get; set; } = default!;
    public ProjectStatsDto ProjectStats { get; set; } = default!;
    public PaymentStatsDto PaymentStats { get; set; } = default!;
    public List<CategoryActivityDto> TopCategories { get; set; } = [];
}

public class UserStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalClients { get; set; }
    public int TotalStores { get; set; }
    public int ActiveUsers { get; set; }
    public int DisabledUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
}

public class ProjectStatsDto
{
    public int Total { get; set; }
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int AwaitingCommissionPayment { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
}

public class PaymentStatsDto
{
    public decimal TotalRevenue { get; set; }
    public int PaidCount { get; set; }
    public int PendingCount { get; set; }
    public int FailedCount { get; set; }
}

public class CategoryActivityDto
{
    public string CategoryName { get; set; } = default!;
    public int ProjectCount { get; set; }
}
