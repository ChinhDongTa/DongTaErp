namespace DongTaErp.Application.Common.DTOs;

public class GoodsIssueLineDto : BaseDto
{
    /// <summary>
    /// Mã phiếu xuất kho
    /// </summary>
    public string GoodsIssueCode { get; set; } = null!;

    /// <summary>
    /// Mã dòng đơn hàng bán
    /// </summary>
    public string SalesOrderLineCode { get; set; } = null!;

    /// <summary>
    /// Mã sản phẩm
    /// </summary>
    public string ProductCode { get; set; } = null!;

    /// <summary>
    /// Tên sản phẩm
    /// </summary>
    public string ProductName { get; set; } = null!;

    /// <summary>
    /// Số lượng xuất
    /// </summary>
    public decimal Quantity { get; set; }
}

public class CreateGoodsIssueLineDto
{
    /// <summary>
    /// Id phiếu xuất kho.
    /// Bắt buộc.
    /// Phải tồn tại trong hệ thống.
    /// </summary>
    public Guid GoodsIssueId { get; set; }

    /// <summary>
    /// Id dòng đơn hàng bán.
    /// Bắt buộc.
    /// Phải tồn tại trong hệ thống.
    /// </summary>
    public Guid SalesOrderLineId { get; set; }

    /// <summary>
    /// Id sản phẩm.
    /// Bắt buộc.
    /// Phải tồn tại trong hệ thống.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Số lượng xuất.
    /// Bắt buộc.
    /// Phải lớn hơn 0.
    /// Không được vượt quá số lượng còn có thể xuất theo đơn hàng.
    /// </summary>
    public decimal Quantity { get; set; }
}

public class UpdateGoodsIssueLineDto
{
    /// <summary>
    /// Id chi tiết phiếu xuất kho.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id phiếu xuất kho.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có phải tồn tại trong hệ thống.
    /// </summary>
    public Guid? GoodsIssueId { get; set; }

    /// <summary>
    /// Id dòng đơn hàng bán.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có phải tồn tại trong hệ thống.
    /// </summary>
    public Guid? SalesOrderLineId { get; set; }

    /// <summary>
    /// Id sản phẩm.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có phải tồn tại trong hệ thống.
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Số lượng xuất.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có phải lớn hơn 0.
    /// Không được vượt quá số lượng còn có thể xuất theo đơn hàng.
    /// </summary>
    public decimal? Quantity { get; set; }
}
