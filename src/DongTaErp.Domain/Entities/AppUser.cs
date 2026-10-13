using Microsoft.AspNetCore.Identity;

namespace DongTaErp.Domain.Entities;

/// <summary>
/// Tài khoản đăng nhập. Kế thừa Identity, không dùng cho công nợ hay kho.
/// </summary>
public class AppUser : IdentityUser
{
    /// <summary>Tên hiển thị trên giao diện.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Tài khoản còn được đăng nhập hay không.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Hồ sơ nhân viên gắn với tài khoản, nếu có.</summary>
    public Employee? Employee { get; set; }
}
