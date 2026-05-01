using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Infrastructure.Persistence.Configurations;

public class UserPolicyViolationConfiguration : IEntityTypeConfiguration<UserPolicyViolation>
{
    public void Configure(EntityTypeBuilder<UserPolicyViolation> builder)
    {
        builder.HasKey(violation => violation.Id);

        builder.Property(violation => violation.Reason)
            .IsRequired();

        builder.Property(violation => violation.OccurredAtUtc)
            .IsRequired();

        builder.HasOne(violation => violation.User)
            .WithMany()
            .HasForeignKey(violation => violation.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProjectRequest>()
            .WithMany()
            .HasForeignKey(violation => violation.ProjectRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProjectProposal>()
            .WithMany()
            .HasForeignKey(violation => violation.ProjectProposalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(violation => new
        {
            violation.UserId,
            violation.ProjectRequestId,
            violation.Reason
        }).IsUnique();
    }
}
