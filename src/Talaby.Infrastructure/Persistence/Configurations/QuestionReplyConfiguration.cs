using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Infrastructure.Persistence.Configurations;

public class QuestionReplyConfiguration : IEntityTypeConfiguration<QuestionReply>
{
    public void Configure(EntityTypeBuilder<QuestionReply> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Content)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(r => r.CreatedAt)
               .IsRequired();

        builder.HasOne(r => r.ProjectQuestion)
               .WithMany(p => p.Replies)
               .HasForeignKey(r => r.ProjectQuestionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Creator)
               .WithMany()
               .HasForeignKey(r => r.CreatorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
