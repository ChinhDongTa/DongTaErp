using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class PurchaseOrderDto : BaseDto
{
    public string Number { get; set; } = null!;
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public string StatusName => Status.ToDisplayName();
    public DateTime? ConfirmedAt { get; set; }
    public string Currency { get; set; } = "VND";
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseOrderLineDto> Lines { get; set; } = [];
}

public class PurchaseOrderLineDto : BaseDto
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductSku { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class CreatePurchaseOrderLineDto
{
    public Guid ProductId { get; set; }
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreatePurchaseOrderDto
{
    public required string Number { get; set; }
    public Guid PartnerId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Today;
    public decimal TaxRate { get; set; } = 0.08m;
    public string? Notes { get; set; }
    public List<CreatePurchaseOrderLineDto> Lines { get; set; } = [];
}

public class UpdatePurchaseOrderDto
{
    public Guid Id { get; set; }
    public string? Number { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? WarehouseId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal? TaxRate { get; set; }
    public string? Notes { get; set; }
}
