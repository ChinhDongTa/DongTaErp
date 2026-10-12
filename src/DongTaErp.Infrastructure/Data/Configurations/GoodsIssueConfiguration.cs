using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DongTaErp.Infrastructure.Data.Configurations;

public class GoodsIssueLineConfiguration : IEntityTypeConfiguration<GoodsIssueLine>
{
    public void Configure(EntityTypeBuilder<GoodsIssueLine> builder)
    {
        builder.ToTable("GoodsIssueLines");

        builder.HasOne(x => x.GoodsIssue)
        .WithMany(g => g.Lines)
        .HasForeignKey(x => x.GoodsIssueId)
        .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.SalesOrderLine)
            .WithMany(x => x.GoodsIssueLines)
            .HasForeignKey(x => x.SalesOrderLineId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
public class GoodsIssueConfiguration : IEntityTypeConfiguration<GoodsIssue>
{
    public void Configure(EntityTypeBuilder<GoodsIssue> builder)
    {
        builder.ToTable("GoodsIssues");
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasOne(x => x.Warehouse)
            .WithMany(w => w.GoodsIssues)
            .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}