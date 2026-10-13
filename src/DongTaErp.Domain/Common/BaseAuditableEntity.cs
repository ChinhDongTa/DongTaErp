namespace DongTaErp.Domain.Common;

/// <summary>
/// Thực thể có thông tin tạo và sửa. Dùng cho hầu hết chứng từ và danh mục.
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity
{
    /// <summary>Thời điểm tạo, UTC.</summary>
    public DateTimeOffset Created { get; set; } = DateTime.UtcNow;

    /// <summary>Người tạo. Thường là user id hoặc tên đăng nhập.</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Thời điểm sửa gần nhất, UTC.</summary>
    public DateTimeOffset LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>Người sửa gần nhất.</summary>
    public string? LastModifiedBy { get; set; }
}
