namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Danh mục phân cấp. Dùng để nhóm sản phẩm hoặc dữ liệu danh mục khác.
/// </summary>
public class Category : BaseAuditableEntity
{
    /// <summary>Loại danh mục.</summary>
    public Guid CategoryTypeId { get; set; }

    /// <summary>Loại danh mục.</summary>
    public CategoryType CategoryType { get; set; } = null!;

    /// <summary>Mã danh mục.</summary>
    [MaxLength(50)]
    public required string Code { get; set; }

    /// <summary>Tên danh mục.</summary>
    [MaxLength(255)]
    public required string Name { get; set; }

    /// <summary>Danh mục cha. null = danh mục gốc.</summary>
    public Guid? ParentId { get; set; }

    /// <summary>Danh mục cha.</summary>
    public Category? Parent { get; set; }

    /// <summary>Các danh mục con.</summary>
    public ICollection<Category> Children { get; set; } = [];
}
