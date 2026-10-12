using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class GoodsIssueDto : BaseDto
{
    public string Code { get; set; } = null!;
    public Guid SalesOrderId { get; set; }
    public string SalesOrderNumber { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public DateTime IssueDate { get; set; }
    public GoodsIssueStatus Status { get; set; }
    public string StatusName => Status.ToDisplayName();
    public DateTime? PostedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? Notes { get; set; }
    public List<GoodsIssueLineDto> Lines { get; set; } = [];
}

public class CreateGoodsIssueDto
{
    public required string Code { get; set; }
    public Guid SalesOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Today;
    public string? Notes { get; set; }
    public List<CreateGoodsIssueLineDto> Lines { get; set; } = [];
}

public class UpdateGoodsIssueDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public Guid? WarehouseId { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? Notes { get; set; }
}
