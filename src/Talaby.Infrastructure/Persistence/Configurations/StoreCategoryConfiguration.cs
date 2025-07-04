using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities;

namespace Talaby.Infrastructure.Persistence.Configurations;

public class StoreCategoryConfiguration : IEntityTypeConfiguration<StoreCategory>
{
    public void Configure(EntityTypeBuilder<StoreCategory> builder)
    {
        builder.Property(u => u.NameAr).IsRequired().HasMaxLength(50);
        builder.Property(u => u.NameEn).IsRequired().HasMaxLength(50);

        builder.HasIndex(p => p.NameAr).IsUnique(); 
        builder.HasIndex(p => p.NameEn).IsUnique(); 
    }
}
