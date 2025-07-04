using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities;

namespace Talaby.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.FirstName).IsRequired();
        //builder.Property(u => u.LastName).IsRequired();
        builder.Property(u => u.Email).IsRequired();

        builder.HasOne(u => u.StoreCategory)
       .WithMany() 
       .HasForeignKey(u => u.StoreCategoryId)
       .OnDelete(DeleteBehavior.Restrict);

        // Create unique index
        builder.HasIndex(c => c.CommercialRegisterNumber).IsUnique();

    }
}
