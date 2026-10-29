
namespace DongTaErp.Application.Common.DTOs;

public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;

    public string UserNameOrEmail { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Roles { get; set; } 
}
public record LoginRequest(string Email, string Password);