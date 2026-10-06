using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace DongTaErp.Web.Security;

public class BlazorAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _session;
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public BlazorAuthStateProvider(ProtectedSessionStorage session)
    {
        _session = session;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await _session.GetAsync<AuthSession>("erp.auth");
            if (!result.Success || result.Value is null || result.Value.ExpiresAt <= DateTime.UtcNow)
                return Anonymous;

            return new AuthenticationState(CreatePrincipal(result.Value));
        }
        catch
        {
            return Anonymous;
        }
    }

    public async Task SignInAsync(AuthSession session)
    {
        await _session.SetAsync("erp.auth", session);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CreatePrincipal(session))));
    }

    public async Task SignOutAsync()
    {
        await _session.DeleteAsync("erp.auth");
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static ClaimsPrincipal CreatePrincipal(AuthSession session)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, session.UserName),
            new Claim(ClaimTypes.GivenName, session.DisplayName),
            new Claim(ClaimTypes.Role, session.Role)
        ], "ErpSession");
        return new ClaimsPrincipal(identity);
    }
}
