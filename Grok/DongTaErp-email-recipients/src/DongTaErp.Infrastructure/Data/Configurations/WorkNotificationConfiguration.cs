using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class PositionDutyConfiguration : IEntityTypeConfiguration<PositionDuty>
{
    public void Configure(EntityTypeBuilder<PositionDuty> builder)
    {
        builder.ToTable("PositionDuties");
        builder.HasIndex(x => new { x.JobPositionId, x.Topic }).IsUnique();
        builder.HasOne(x => x.JobPosition).WithMany().HasForeignKey(x => x.JobPositionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkAssignmentConfiguration : IEntityTypeConfiguration<WorkAssignment>
{
    public void Configure(EntityTypeBuilder<WorkAssignment> builder)
    {
        builder.ToTable("WorkAssignments");
        builder.HasIndex(x => new { x.EmployeeId, x.Topic }).IsUnique();
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
    }
}
