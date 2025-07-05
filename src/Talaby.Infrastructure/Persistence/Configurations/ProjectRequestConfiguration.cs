using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Infrastructure.Persistence.Configurations;
public class ProjectRequestConfiguration : IEntityTypeConfiguration<ProjectRequest>
{

    public void Configure(EntityTypeBuilder<ProjectRequest> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
               .HasMaxLength(200);

        builder.Property(p => p.Description)
               .IsRequired();

        builder.Property(pr => pr.MinBudget)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(pr => pr.MaxBudget)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.StoreCategory)
               .WithMany()
               //.WithMany(c => c.ProjectRequests)
               .HasForeignKey(p => p.StoreCategoryId);

        builder.HasOne(p => p.Creator)
               .WithMany()
               .HasForeignKey(p => p.CreatorId);
    }
}

