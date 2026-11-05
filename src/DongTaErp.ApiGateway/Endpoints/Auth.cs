using System.Security.Claims;
using DongTaErp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Auth : EndpointGroupBase
{
    public override string? GroupName => "Auth";

    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(Login, "/login").AllowAnonymous();
        group.MapPost(Refresh, "/refresh").AllowAnonymous();
        group.MapGet(Me, "/me").RequireAuthorization();
    }

    public async Task<IResult> Login([FromServices] IIdentityService identity, [FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await identity.LoginAsync(request, ct);
        if (!result.Succeeded || result.Value is null)
            return Result<TokenResponse>.Unauthorized("Sai email hoặc mật khẩu.").ToHttpResult();
        return Result<TokenResponse>.Success(Issue(identity, result.Value)).ToHttpResult();
    }

    public IResult Refresh([FromServices] IIdentityService identity, [FromBody] RefreshRequest request)
    {
        var principal = identity.ValidateToken(request.RefreshToken);
        var userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (principal is null || string.IsNullOrEmpty(userId))
            return Result<TokenResponse>.Unauthorized("Refresh token không hợp lệ.").ToHttpResult();

        var user = new UserDto
        {
            Id = userId,
            UserNameOrEmail = principal.FindFirstValue(ClaimTypes.Email) ?? "",
            Roles = string.Join(",", principal.FindAll(ClaimTypes.Role).Select(x => x.Value))
        };
        return Result<TokenResponse>.Success(Issue(identity, user)).ToHttpResult();
    }

    public IResult Me(ClaimsPrincipal user)
    {
        var dto = new UserDto
        {
            Id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            UserNameOrEmail = user.FindFirstValue(ClaimTypes.Email) ?? user.Identity?.Name ?? "",
            Roles = string.Join(",", user.FindAll(ClaimTypes.Role).Select(x => x.Value))
        };
        return Result<UserDto>.Success(dto).ToHttpResult();
    }

    private static TokenResponse Issue(IIdentityService identity, UserDto user) => new()
    {
        AccessToken = identity.GenerateAccessToken(user, 60),
        RefreshToken = identity.GenerateRefreshToken(user),
        AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
        User = user
    };
}
