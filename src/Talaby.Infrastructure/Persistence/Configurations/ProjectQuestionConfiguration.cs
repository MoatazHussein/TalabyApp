using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Infrastructure.Persistence.Configurations;

public class ProjectQuestionConfiguration : IEntityTypeConfiguration<ProjectQuestion>
{
    public void Configure(EntityTypeBuilder<ProjectQuestion> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Content)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(p => p.CreatedAt)
               .IsRequired();

        builder.HasOne(p => p.Creator)
               .WithMany()
               .HasForeignKey(p => p.CreatorId)
               .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(p => p.ProjectRequest)
               .WithMany(r => r.Questions)
               //.WithMany()
               .HasForeignKey(p => p.ProjectRequestId)
               .OnDelete(DeleteBehavior.Cascade); 
    }
}
