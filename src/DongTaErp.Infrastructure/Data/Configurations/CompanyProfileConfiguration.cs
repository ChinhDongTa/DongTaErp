using DongTaErp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.ToTable("CompanyProfiles");
    }
}
