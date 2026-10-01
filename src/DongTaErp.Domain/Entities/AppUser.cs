using Microsoft.AspNetCore.Identity;
namespace DongTaErp.Domain.Entities;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Employee? Employee { get; set; }
}