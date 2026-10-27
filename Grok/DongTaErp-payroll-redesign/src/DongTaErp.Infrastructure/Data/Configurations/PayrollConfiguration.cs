using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
{
    public void Configure(EntityTypeBuilder<Payroll> builder)
    {
        builder.ToTable("Payrolls");
        builder.HasIndex(x => new { x.EmployeeId, x.PeriodStartDate, x.PeriodEndDate }).IsUnique();
        builder.HasOne(x => x.Employee).WithMany(x => x.PayrollRecords).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.GrossAmount).HasPrecision(18, 2);
        builder.Property(x => x.DeductionAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
    }
}

public class SalaryComponentConfiguration : IEntityTypeConfiguration<SalaryComponent>
{
    public void Configure(EntityTypeBuilder<SalaryComponent> builder)
    {
        builder.ToTable("SalaryComponents");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public class EmployeeCompensationConfiguration : IEntityTypeConfiguration<EmployeeCompensation>
{
    public void Configure(EntityTypeBuilder<EmployeeCompensation> builder)
    {
        builder.ToTable("EmployeeCompensations");
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.Employee).WithMany(x => x.Compensations).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SalaryComponent).WithMany().HasForeignKey(x => x.SalaryComponentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.EmployeeId, x.SalaryComponentId, x.EffectiveFrom });
    }
}

public class PayrollLineConfiguration : IEntityTypeConfiguration<PayrollLine>
{
    public void Configure(EntityTypeBuilder<PayrollLine> builder)
    {
        builder.ToTable("PayrollLines");
        builder.Property(x => x.Quantity).HasPrecision(18, 2);
        builder.Property(x => x.Rate).HasPrecision(18, 2);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.HasOne(x => x.Payroll).WithMany(x => x.Lines).HasForeignKey(x => x.PayrollId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SalaryComponent).WithMany().HasForeignKey(x => x.SalaryComponentId).OnDelete(DeleteBehavior.Restrict);
    }
}
