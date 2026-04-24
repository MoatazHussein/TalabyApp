using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities.Payments;

namespace Talaby.Infrastructure.Persistence.Configurations;

public sealed class ProjectCommissionPaymentAttemptConfiguration : IEntityTypeConfiguration<ProjectCommissionPaymentAttempt>
{
    public void Configure(EntityTypeBuilder<ProjectCommissionPaymentAttempt> builder)
    {
        builder.ToTable("ProjectCommissionPaymentAttempts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProviderName)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ProviderChargeId)
            .HasMaxLength(128);

        builder.HasIndex(x => x.ProviderChargeId);

        builder.Property(x => x.ProviderTransactionReference)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.ProviderTransactionReference)
            .IsUnique();

        builder.Property(x => x.ProviderPaymentReference)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.IdempotencyKey)
            .IsUnique();

        builder.Property(x => x.RequestedAmount)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.RequestedCurrency)
            .HasMaxLength(3)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CheckoutUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ProjectCommissionPaymentId);
    }
}