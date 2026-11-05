using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("Announcements");
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.Content).HasMaxLength(4000);
        builder.HasIndex(x => x.Created);
    }
}

public class AnnouncementRecipientConfiguration : IEntityTypeConfiguration<AnnouncementRecipient>
{
    public void Configure(EntityTypeBuilder<AnnouncementRecipient> builder)
    {
        builder.ToTable("AnnouncementRecipients");
        builder.HasIndex(x => new { x.AnnouncementId, x.EmployeeId }).IsUnique();
        builder.HasOne(x => x.Announcement).WithMany(x => x.Recipients).HasForeignKey(x => x.AnnouncementId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}
