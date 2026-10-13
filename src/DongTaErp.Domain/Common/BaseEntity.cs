namespace DongTaErp.Domain.Common;

/// <summary>
/// Thực thể gốc. Mọi entity nghiệp vụ kế thừa class này.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Khóa chính.</summary>
    [Key]
    public Guid Id { get; set; } = new Guid();

    /// <summary>Xóa mềm. true = không còn dùng, không xóa vật lý.</summary>
    public bool IsDeleted { get; set; } = false;
}
