using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class GoodsReceiptDto : BaseDto
{
    public string Code { get; set; } = null!;
    public Guid PurchaseOrderId { get; set; }
    public string PurchaseOrderNumber { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public DateTime ReceiptDate { get; set; }
    public GoodsReceiptStatus Status { get; set; }
    public string StatusName => Status.ToDisplayName();
    public DateTime? PostedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? SupplierRef { get; set; }
    public string? Notes { get; set; }
    public List<GoodsReceiptLineDto> Lines { get; set; } = [];
}

public class GoodsReceiptLineDto : BaseDto
{
    public Guid GoodsReceiptId { get; set; }
    public Guid PurchaseOrderLineId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductSku { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string? Note { get; set; }
}

public class CreateGoodsReceiptLineDto
{
    public Guid PurchaseOrderLineId { get; set; }
    public decimal Quantity { get; set; }
    public string? Note { get; set; }
}

public class CreateGoodsReceiptDto
{
    public required string Code { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime ReceiptDate { get; set; } = DateTime.Today;
    public string? SupplierRef { get; set; }
    public string? Notes { get; set; }
    public List<CreateGoodsReceiptLineDto> Lines { get; set; } = [];
}

public class UpdateGoodsReceiptDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public Guid? WarehouseId { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string? SupplierRef { get; set; }
    public string? Notes { get; set; }
}
