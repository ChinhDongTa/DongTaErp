using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class JobPositionConfiguration : IEntityTypeConfiguration<JobPosition>
{
    public void Configure(EntityTypeBuilder<JobPosition> builder)
    {
        builder.ToTable("JobPositions");

        builder.HasIndex(x => x.Code)
        .IsUnique();
    }
}
