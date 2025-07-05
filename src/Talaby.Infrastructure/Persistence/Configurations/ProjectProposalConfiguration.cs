using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Infrastructure.Persistence.Configurations;

public class ProjectProposalConfiguration : IEntityTypeConfiguration<ProjectProposal>
{
    public void Configure(EntityTypeBuilder<ProjectProposal> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Content)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(p => p.ProposedAmount)
               .HasPrecision(18, 2) 
               .IsRequired();

        builder.Property(p => p.CreatedAt)
               .IsRequired();

        builder.HasOne(p => p.Creator)
               .WithMany()
               .HasForeignKey(p => p.CreatorId)
               .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(p => p.ProjectRequest)
               .WithMany(r => r.Proposals)
               //.WithMany()
               .HasForeignKey(p => p.ProjectRequestId)
               .OnDelete(DeleteBehavior.Cascade); 
    }
}
