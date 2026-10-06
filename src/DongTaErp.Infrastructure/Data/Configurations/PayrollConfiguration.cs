using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

/// <summary>
/// Cấu hình Entity Framework cho Payroll
/// </summary>
public class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
{
    public void Configure(EntityTypeBuilder<Payroll> builder)
    {
        builder.ToTable("Payrolls");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.EmployeeId)
            .IsRequired();

        builder.Property(p => p.DeductionDetails)
            .HasMaxLength(1000);

        builder.Property(p => p.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        // Navigation: Payroll -> Employee
        builder.HasOne(p => p.Employee)
            .WithMany(e => e.PayrollRecords)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.EmployeeId);
        builder.HasIndex(p => p.PeriodStartDate);
        builder.HasIndex(p => p.PeriodEndDate);
        builder.HasIndex(p => p.PaymentStatus);
        builder.HasIndex(p => new { p.EmployeeId, p.PeriodStartDate, p.PeriodEndDate });
    }

}
