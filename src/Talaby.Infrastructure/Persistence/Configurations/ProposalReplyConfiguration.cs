using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Infrastructure.Persistence.Configurations;

public class ProposalReplyConfiguration : IEntityTypeConfiguration<ProposalReply>
{
    public void Configure(EntityTypeBuilder<ProposalReply> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Content)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(r => r.CreatedAt)
               .IsRequired();

        builder.HasOne(r => r.ProjectProposal)
               .WithMany(p => p.Replies)
               .HasForeignKey(r => r.ProjectProposalId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Creator)
               .WithMany()
               .HasForeignKey(r => r.CreatorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
