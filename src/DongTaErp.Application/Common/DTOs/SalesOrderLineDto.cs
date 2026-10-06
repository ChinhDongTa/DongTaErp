namespace DongTaErp.Application.Common.DTOs;

public class SalesOrderLineDto : BaseDto
{
    public string SalesOrderNumber { get; set; } = null!;

    public string ProductSku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal Qty { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}

public class CreateSalesOrderLineDto
{
    /// <summary>
    /// Đơn hàng bán.
    /// Bắt buộc.
    /// </summary>
    public Guid SalesOrderId { get; set; }

    /// <summary>
    /// Sản phẩm.
    /// Bắt buộc.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Số lượng.
    /// Phải lớn hơn 0.
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// Đơn giá.
    /// Phải lớn hơn hoặc bằng 0.
    /// </summary>
    public decimal UnitPrice { get; set; }
}

public class UpdateSalesOrderLineDto
{
    public Guid Id { get; set; }

    public Guid? SalesOrderId { get; set; }

    public Guid? ProductId { get; set; }

    public decimal? Qty { get; set; }

    public decimal? UnitPrice { get; set; }
}