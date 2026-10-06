using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Common.DTOs;

public class SalesOrderDto : BaseDto
{
    public string Number { get; set; } = null!;

    public string PartnerName { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public DateTime OrderDate { get; set; }
    public SalesOrderStatus Status { get; set; }
    public string StatusName => Status.ToDisplayName();

    public string? Notes { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TaxRate { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal Total { get; set; }

    public DateTime? ConfirmedAt { get; set; }
}

public class CreateSalesOrderDto
{
    /// <summary>
    /// Số đơn hàng.
    /// Bắt buộc.
    /// Tối đa 30 ký tự.
    /// Không được trùng.
    /// </summary>
    public required string Number { get; set; }

    /// <summary>
    /// Khách hàng.
    /// Bắt buộc.
    /// </summary>
    public Guid PartnerId { get; set; }

    /// <summary>
    /// Kho mặc định.
    /// Bắt buộc.
    /// </summary>
    public Guid WarehouseId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Today;

    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Draft;

    public string? Notes { get; set; }

    public decimal TaxRate { get; set; } = 0.08m;
}

public class UpdateSalesOrderDto
{
    public Guid Id { get; set; }

    public string? Number { get; set; }

    public Guid? PartnerId { get; set; }

    public Guid? WarehouseId { get; set; }

    public DateTime? OrderDate { get; set; }

    public SalesOrderStatus? Status { get; set; }

    public string? Notes { get; set; }

    public decimal? TaxRate { get; set; }

    public DateTime? ConfirmedAt { get; set; }
}