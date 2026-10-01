using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");

        builder.Property(x => x.Days)
        .HasPrecision(8, 2);

        builder.HasOne(x => x.Employee)
        .WithMany(e => e.LeaveRequests)
        .HasForeignKey(x => x.EmployeeId);
    }
}