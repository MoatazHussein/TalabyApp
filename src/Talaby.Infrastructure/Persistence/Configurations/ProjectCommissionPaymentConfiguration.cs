using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talaby.Domain.Entities.Payments;


namespace Talaby.Infrastructure.Persistence.Configurations;

public sealed class ProjectCommissionPaymentConfiguration : IEntityTypeConfiguration<ProjectCommissionPayment>
{
    public void Configure(EntityTypeBuilder<ProjectCommissionPayment> builder)
    {
        builder.ToTable("ProjectCommissionPayments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProposalAmountSnapshot)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.CommissionPercentage)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.CommissionAmount)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.PaidAtUtc);

        builder.HasIndex(x => x.ProjectRequestId)
            .IsUnique();

        builder.HasIndex(x => x.ProjectProposalId)
            .IsUnique();

        builder.HasOne(x => x.ProjectRequest)
            .WithOne()
            .HasForeignKey<ProjectCommissionPayment>(x => x.ProjectRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProjectProposal)
            .WithOne()
            .HasForeignKey<ProjectCommissionPayment>(x => x.ProjectProposalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PaymentAttempts)
            .WithOne(x => x.ProjectCommissionPayment)
            .HasForeignKey(x => x.ProjectCommissionPaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.PaymentAttempts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
