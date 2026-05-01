using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Entities;
using Talaby.Domain.Entities.Payments;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Infrastructure.Persistence;

public class TalabyDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public TalabyDbContext(DbContextOptions<TalabyDbContext> options)
        : base(options) { }

    public DbSet<StoreCategory> StoreCategories { get; set; }
    public DbSet<ProjectRequest> ProjectRequests { get; set; }
    public DbSet<ProjectProposal> ProjectProposals { get; set; }
    public DbSet<ProposalReply> ProposalReplies { get; set; }
    public DbSet<ProjectQuestion> ProjectQuestions { get; set; }
    public DbSet<QuestionReply> QuestionReplies { get; set; }
    public DbSet<ProjectCommissionPayment> ProjectCommissionPayments { get; set; }
    public DbSet<ProjectCommissionPaymentAttempt> ProjectCommissionPaymentAttempts { get; set; }
    public DbSet<UserPolicyViolation> UserPolicyViolations { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); 
        builder.ApplyConfigurationsFromAssembly(typeof(TalabyDbContext).Assembly);


    }
}


