namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Loại danh mục, ví dụ nhóm hàng, nhóm chi phí. Category thuộc một loại.
/// </summary>
public class CategoryType : BaseAuditableEntity
{
    /// <summary>Mã loại danh mục, duy nhất.</summary>
    [MaxLength(50)]
    public required string Code { get; set; }

    /// <summary>Tên loại danh mục.</summary>
    [MaxLength(255)]
    public required string Name { get; set; }

    /// <summary>Các danh mục thuộc loại này.</summary>
    public ICollection<Category> Categories { get; set; } = [];
}
