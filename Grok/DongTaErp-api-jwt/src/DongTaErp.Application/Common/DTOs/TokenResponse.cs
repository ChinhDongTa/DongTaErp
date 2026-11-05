namespace DongTaErp.Application.Common.DTOs;

public class TokenResponse
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime AccessTokenExpiresAt { get; set; }
    public UserDto User { get; set; } = new();
}

public record RefreshRequest(string RefreshToken);
