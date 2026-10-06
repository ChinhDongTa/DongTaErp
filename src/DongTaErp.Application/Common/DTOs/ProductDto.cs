namespace DongTaErp.Application.Common.DTOs;

public class ProductDto : BaseDto
{
    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? CategoryName { get; set; }

    public string Unit { get; set; } = null!;

    public decimal CostPrice { get; set; }

    public decimal SalePrice { get; set; }

    public decimal MinStock { get; set; }

    public bool IsActive { get; set; }
}

public class CreateProductDto
{
    /// <summary>
    /// SKU/Mã sản phẩm.
    /// Bắt buộc.
    /// Tối đa 40 ký tự.
    /// Không được trùng.
    /// </summary>
    public required string Sku { get; set; }

    /// <summary>
    /// Tên sản phẩm.
    /// Bắt buộc.
    /// Tối đa 200 ký tự.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Danh mục.
    /// Không bắt buộc.
    /// Nếu có phải tồn tại trong hệ thống.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Đơn vị tính.
    /// Bắt buộc.
    /// Tối đa 20 ký tự.
    /// </summary>
    public required string Unit { get; set; }

    /// <summary>
    /// Giá vốn.
    /// Phải lớn hơn hoặc bằng 0.
    /// </summary>
    public decimal CostPrice { get; set; }

    /// <summary>
    /// Giá bán.
    /// Phải lớn hơn hoặc bằng 0.
    /// </summary>
    public decimal SalePrice { get; set; }

    /// <summary>
    /// Tồn kho tối thiểu.
    /// Phải lớn hơn hoặc bằng 0.
    /// </summary>
    public decimal MinStock { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateProductDto
{
    public Guid Id { get; set; }

    public string? Sku { get; set; }

    public string? Name { get; set; }

    public Guid? CategoryId { get; set; }

    public string? Unit { get; set; }

    public decimal? CostPrice { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? MinStock { get; set; }

    public bool? IsActive { get; set; }
}