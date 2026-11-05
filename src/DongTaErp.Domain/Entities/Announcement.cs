using DongTaErp.Domain.Enums;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Thông báo nội bộ: tăng lương, giảm lương, tăng ca, chỉ đạo.
/// </summary>
public class Announcement : BaseAuditableEntity
{
    /// <summary>Tiêu đề.</summary>
    [MaxLength(200)]
    public required string Title { get; set; }

    /// <summary>Nội dung.</summary>
    [MaxLength(4000)]
    public required string Content { get; set; }

    /// <summary>Loại thông báo.</summary>
    public AnnouncementType Type { get; set; } = AnnouncementType.General;

    /// <summary>Người gửi, id tài khoản.</summary>
    [MaxLength(450)]
    public string? SenderUserId { get; set; }

    /// <summary>Tên người gửi lúc gửi.</summary>
    [MaxLength(160)]
    public string SenderName { get; set; } = "";

    /// <summary>Người nhận.</summary>
    public ICollection<AnnouncementRecipient> Recipients { get; set; } = [];
}

/// <summary>
/// Người nhận một thông báo.
/// </summary>
public class AnnouncementRecipient : BaseAuditableEntity
{
    /// <summary>Thông báo.</summary>
    public Guid AnnouncementId { get; set; }

    /// <summary>Thông báo.</summary>
    public Announcement Announcement { get; set; } = null!;

    /// <summary>Nhân viên nhận.</summary>
    public Guid EmployeeId { get; set; }

    /// <summary>Nhân viên nhận.</summary>
    public Employee Employee { get; set; } = null!;

    /// <summary>Đã đọc.</summary>
    public bool IsRead { get; set; }
}
