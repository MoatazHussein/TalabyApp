using Microsoft.EntityFrameworkCore;
using Talaby.Application.Features.Dashboard.Queries.Admin;
using Talaby.Application.Features.Dashboard.Queries.Client;
using Talaby.Application.Features.Dashboard.Queries.Store;
using Talaby.Domain.Enums;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories;

public class DashboardReadRepository(TalabyDbContext context) :
    IAdminDashboardReadRepository,
    IClientDashboardReadRepository,
    IStoreDashboardReadRepository
{
    public async Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var firstOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var now = DateTimeOffset.UtcNow;

        var userCounts = await context.Users
            .GroupBy(u => u.UserType)
            .Select(g => new { UserType = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalUsers = userCounts.Sum(x => x.Count);

        var disabledUsers = await context.Users
            .CountAsync(u => u.LockoutEnd.HasValue && u.LockoutEnd > now, cancellationToken);

        var newUsersThisMonth = await context.Users
            .CountAsync(u => u.CreatedAt >= firstOfMonth, cancellationToken);

        var projectCounts = await context.ProjectRequests
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var paymentCounts = await context.ProjectCommissionPayments
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalRevenue = await context.ProjectCommissionPayments
            .Where(p => p.Status == ProjectCommissionPaymentStatus.Paid)
            .SumAsync(p => p.CommissionAmount, cancellationToken);

        var topCategories = await context.ProjectRequests
            .GroupBy(p => p.StoreCategory.NameEn)
            .Select(g => new CategoryActivityDto
            {
                CategoryName = g.Key,
                ProjectCount = g.Count()
            })
            .OrderByDescending(c => c.ProjectCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new AdminDashboardDto
        {
            UserStats = new UserStatsDto
            {
                TotalUsers = totalUsers,
                TotalClients = userCounts.FirstOrDefault(x => x.UserType == UserType.Client)?.Count ?? 0,
                TotalStores = userCounts.FirstOrDefault(x => x.UserType == UserType.Store)?.Count ?? 0,
                ActiveUsers = totalUsers - disabledUsers,
                DisabledUsers = disabledUsers,
                NewUsersThisMonth = newUsersThisMonth
            },
            ProjectStats = new ProjectStatsDto
            {
                Total = projectCounts.Sum(x => x.Count),
                Open = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.Open)?.Count ?? 0,
                InProgress = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.InProgress)?.Count ?? 0,
                AwaitingCommissionPayment = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.AwaitingCommissionPayment)?.Count ?? 0,
                Completed = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.Completed)?.Count ?? 0,
                Cancelled = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.Cancelled)?.Count ?? 0
            },
            PaymentStats = new PaymentStatsDto
            {
                TotalRevenue = totalRevenue,
                PaidCount = paymentCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Paid)?.Count ?? 0,
                PendingCount = paymentCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Pending)?.Count ?? 0,
                FailedCount = paymentCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Failed)?.Count ?? 0
            },
            TopCategories = topCategories
        };
    }

    public async Task<ClientDashboardDto> GetClientDashboardAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var clientProjects = context.ProjectRequests
            .AsNoTracking()
            .Where(p => p.CreatorId == clientId);

        var projectCounts = await clientProjects
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var proposalCounts = await clientProjects
            .SelectMany(p => p.Proposals)
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var paymentCounts = await context.ProjectCommissionPayments
            .AsNoTracking()
            .Where(p => clientProjects.Any(project => project.Id == p.ProjectRequestId))
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalCommissionPaid = await context.ProjectCommissionPayments
            .AsNoTracking()
            .Where(p => clientProjects.Any(project => project.Id == p.ProjectRequestId)
                        && p.Status == ProjectCommissionPaymentStatus.Paid)
            .SumAsync(p => (decimal?)p.CommissionAmount, cancellationToken) ?? 0m;

        var recentProjects = await clientProjects
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new ClientRecentProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                CreatedAt = p.CreatedAt,
                Status = p.Status,
                StoreCategoryId = p.StoreCategoryId,
                StoreCategoryName = p.StoreCategory.NameEn,
                ProposalsCount = p.Proposals.Count
            })
            .ToListAsync(cancellationToken);

        var topCategories = await clientProjects
            .GroupBy(p => new { p.StoreCategoryId, p.StoreCategory.NameEn })
            .Select(g => new ClientCategoryActivityDto
            {
                StoreCategoryId = g.Key.StoreCategoryId,
                CategoryName = g.Key.NameEn,
                ProjectCount = g.Count()
            })
            .OrderByDescending(c => c.ProjectCount)
            .ThenBy(c => c.CategoryName)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new ClientDashboardDto
        {
            ProjectStats = new ClientProjectStatsDto
            {
                Total = projectCounts.Sum(x => x.Count),
                Open = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.Open)?.Count ?? 0,
                InProgress = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.InProgress)?.Count ?? 0,
                AwaitingCommissionPayment = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.AwaitingCommissionPayment)?.Count ?? 0,
                Completed = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.Completed)?.Count ?? 0,
                Cancelled = projectCounts.FirstOrDefault(x => x.Status == ProjectRequestStatus.Cancelled)?.Count ?? 0
            },
            ProposalStats = new ClientProposalStatsDto
            {
                Total = proposalCounts.Sum(x => x.Count),
                Pending = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Pending)?.Count ?? 0,
                Accepted = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Accepted)?.Count ?? 0,
                Rejected = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Rejected)?.Count ?? 0,
                Cancelled = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Cancelled)?.Count ?? 0,
                Completed = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Completed)?.Count ?? 0
            },
            PaymentStats = new ClientPaymentStatsDto
            {
                TotalCommissionPaid = totalCommissionPaid,
                PendingCount = paymentCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Pending)?.Count ?? 0,
                InitiatedCount = paymentCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Initiated)?.Count ?? 0,
                PaidCount = paymentCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Paid)?.Count ?? 0,
                FailedCount = paymentCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Failed)?.Count ?? 0
            },
            RecentProjects = recentProjects,
            TopCategories = topCategories
        };
    }

    public async Task<StoreDashboardDto> GetStoreDashboardAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        var storeProposals = context.ProjectProposals
            .AsNoTracking()
            .Where(p => p.CreatorId == storeId);

        var proposalCounts = await storeProposals
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var workCounts = await storeProposals
            .Where(p => p.Status == ProjectProposalStatus.Accepted || p.Status == ProjectProposalStatus.Completed)
            .GroupBy(p => p.ProjectRequest!.Status)
            .Select(g => new { ProjectStatus = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var acceptedProposalAmount = await storeProposals
            .Where(p => p.Status == ProjectProposalStatus.Accepted || p.Status == ProjectProposalStatus.Completed)
            .SumAsync(p => (decimal?)p.ProposedAmount, cancellationToken) ?? 0m;

        var completedProposalAmount = await storeProposals
            .Where(p => p.Status == ProjectProposalStatus.Completed)
            .SumAsync(p => (decimal?)p.ProposedAmount, cancellationToken) ?? 0m;

        var averageAcceptedProposalAmount = await storeProposals
            .Where(p => p.Status == ProjectProposalStatus.Accepted || p.Status == ProjectProposalStatus.Completed)
            .AverageAsync(p => (decimal?)p.ProposedAmount, cancellationToken) ?? 0m;

        var commissionCounts = await context.ProjectCommissionPayments
            .AsNoTracking()
            .Where(p => storeProposals.Any(proposal => proposal.Id == p.ProjectProposalId))
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalCommissionPaid = await context.ProjectCommissionPayments
            .AsNoTracking()
            .Where(p => storeProposals.Any(proposal => proposal.Id == p.ProjectProposalId)
                        && p.Status == ProjectCommissionPaymentStatus.Paid)
            .SumAsync(p => (decimal?)p.CommissionAmount, cancellationToken) ?? 0m;

        var recentProposals = await storeProposals
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new StoreRecentProposalDto
            {
                Id = p.Id,
                ProjectRequestId = p.ProjectRequestId,
                ProjectTitle = p.ProjectRequest!.Title,
                ProposedAmount = p.ProposedAmount,
                CreatedAt = p.CreatedAt,
                ProposalStatus = p.Status,
                ProjectStatus = p.ProjectRequest.Status
            })
            .ToListAsync(cancellationToken);

        return new StoreDashboardDto
        {
            ProposalStats = new StoreProposalStatsDto
            {
                Total = proposalCounts.Sum(x => x.Count),
                Pending = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Pending)?.Count ?? 0,
                Accepted = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Accepted)?.Count ?? 0,
                Rejected = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Rejected)?.Count ?? 0,
                Cancelled = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Cancelled)?.Count ?? 0,
                Completed = proposalCounts.FirstOrDefault(x => x.Status == ProjectProposalStatus.Completed)?.Count ?? 0
            },
            WorkStats = new StoreWorkStatsDto
            {
                Active = workCounts.FirstOrDefault(x => x.ProjectStatus == ProjectRequestStatus.InProgress)?.Count ?? 0,
                AwaitingCommissionPayment = workCounts.FirstOrDefault(x => x.ProjectStatus == ProjectRequestStatus.AwaitingCommissionPayment)?.Count ?? 0,
                Completed = workCounts.FirstOrDefault(x => x.ProjectStatus == ProjectRequestStatus.Completed)?.Count ?? 0
            },
            EarningsStats = new StoreEarningsStatsDto
            {
                TotalAcceptedProposalAmount = acceptedProposalAmount,
                TotalCompletedProposalAmount = completedProposalAmount,
                AverageAcceptedProposalAmount = averageAcceptedProposalAmount
            },
            CommissionStats = new StoreCommissionStatsDto
            {
                TotalCommissionPaid = totalCommissionPaid,
                PendingCount = commissionCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Pending)?.Count ?? 0,
                InitiatedCount = commissionCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Initiated)?.Count ?? 0,
                PaidCount = commissionCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Paid)?.Count ?? 0,
                FailedCount = commissionCounts.FirstOrDefault(x => x.Status == ProjectCommissionPaymentStatus.Failed)?.Count ?? 0
            },
            RecentProposals = recentProposals
        };
    }
}
