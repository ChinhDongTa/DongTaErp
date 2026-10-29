using System.Security.Claims;

namespace DongTaErp.Application.Common.Interfaces;

public interface IIdentityService
{
    //=================================User===========================================
    Task<Result> AddToRoleAsync(string userId, string role, CancellationToken ct=default);
    Task<Result<string?>> GetUserNameAsync(string userId, CancellationToken ct = default);
    Task<Result<List<UserDto>>> GetAllUserDtoAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task<Result<List<UserDto>>> SearchAsync(SimpleSearch simpleSearch,CancellationToken ct = default);

    Task<Result<bool>> IsInRoleAsync(string userId, string role, CancellationToken ct = default);

    Task<Result<bool>> AuthorizeAsync(string userId, string policyName, CancellationToken ct = default);

    Task<Result<string>> CreateUserAsync(string email, string password, CancellationToken ct = default);

    Task<Result> DeleteUserAsync(string userId, CancellationToken ct = default);
    Task<Result> RemoveFromRoleAsync(string userId, string role, CancellationToken ct = default);

    Task<Result<UserDto?>> LoginAsync(LoginRequest request, CancellationToken ct = default);

    //===================================Role=======================================

    Task<Result<List<RoleDto>>> GetAllRoleDtoAsync(CancellationToken ct = default);
    Task<Result<string>> CreateRoleAsync(string roleName, CancellationToken ct = default);
    Task<Result> UpdateRoleAsync(string roleId, string newRoleName, CancellationToken ct = default);
    Task<Result> DeleteRoleAsync(string roleId, CancellationToken ct = default);

    //====================================Token==================================
    string GenerateAccessToken(UserDto user, int expiresInMinutes = 15);

    string GenerateRefreshToken(UserDto user, int expiresInDays = 7);
    ClaimsPrincipal? ValidateToken(string token);
}