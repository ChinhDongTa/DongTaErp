using DongTaErp.Application.Common.DTOs;
using DongTaErp.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DongTaErp.Infrastructure.Repositories;

public class IdentityService(UserManager<AppUser> userManager,
                             RoleManager<IdentityRole> roleManager,
                             IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
                             IAuthorizationService authorizationService,
                             IConfiguration configuration,
                             ILogger<IdentityService> logger) : IIdentityService
{
    private readonly SymmetricSecurityKey _key = new(Encoding.ASCII.GetBytes(configuration["Jwt:SecretKey"]
        ?? throw new InvalidOperationException("Jwt:SecretKey not configured")));
    public async Task<Result> AddToRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            return Result.NotFound(role);
        }
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<bool>.NotFound("User");
        }
        var result = await userManager.AddToRoleAsync(user, role);
        return result.Succeeded ? Result.Success(ResultStatus.NoContent) : Result<bool>.Error([..GetIdentityErrors(result)]);
    }

    public async Task<Result<bool>> AuthorizeAsync(string userId, string policyName, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Result<bool>.NotFound("User");
        }

        var principal = await userClaimsPrincipalFactory.CreateAsync(user);

        var result = await authorizationService.AuthorizeAsync(principal, policyName);

        return Result<bool>.Success(result.Succeeded);
    }

    public async Task<Result<string>> CreateRoleAsync(string roleName, CancellationToken ct = default)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var role = new IdentityRole(roleName);
            var identityResult = await roleManager.CreateAsync(role);
            return identityResult.Succeeded
                ? Result<string>.Success(role.Id)
                : Result<string>.Error([.. GetIdentityErrors(identityResult)]);
        }
        return Result<string>.Error("Không thể tạo role");
    }

    public async Task<Result<string>> CreateUserAsync(string email, string password, CancellationToken ct = default)
    {
        //1. Check if user already exists
        var existingUser = await userManager.FindByNameAsync(email);
        if (existingUser != null)
        {
            return Result<string>.Error($"{email} đã có rồi");
        }
        var user = new AppUser
        {
            UserName = email,
            Email = email,
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            logger.LogInformation("User created successfully: {UserId}", user.Id);
            string defaultRole = "Staff";
            var addToRoleResult = await AddToRoleAsync( user.Id ,defaultRole, ct);
            if (addToRoleResult.Succeeded)
                logger.LogInformation($"User added to role {defaultRole} successfully!");
            return Result<string>.Success(user.Id, "Tạo User thành công !");
        }

        return Result<string>.Error([.. GetIdentityErrors(result)]);
    }

    public async Task<Result> DeleteRoleAsync(string roleId, CancellationToken ct = default)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if(role!=null)
        {
            var result = await roleManager.DeleteAsync(role);
            return result.Succeeded ? Result.Success(ResultStatus.NoContent) : Result.Error([..GetIdentityErrors(result)]);
        }
        return Result.Error("Không thể xóa Role");
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.NotFound("User");
        }

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded ? Result.Success(ResultStatus.NoContent) : Result.Error(GetIdentityErrors(result).ToArray());
    }

    public string GenerateAccessToken(UserDto user, int expiresInMinutes = 15)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id),
            new (ClaimTypes.Email, user.UserNameOrEmail),
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        };

        // ✅ Add roles
        foreach (var role in user.Roles ?? [])
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            SigningCredentials = credentials
        };

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityTokenHandler().CreateToken(tokenDescriptor));
    }

    public string GenerateRefreshToken(UserDto user, int expiresInDays = 7)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id),
            new (JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        };

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(expiresInDays),
            SigningCredentials = credentials
        };

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityTokenHandler().CreateToken(tokenDescriptor));
    }

    public async Task<Result<List<RoleDto>>> GetAllRoleDtoAsync(CancellationToken ct = default)
    => Result<List<RoleDto>>.Success((await roleManager.Roles.Select(x => new RoleDto() { Id= x.Id,Name= x.Name! }).ToListAsync(ct)));

    public async Task<Result<List<UserDto>>> GetAllUserDto(CancellationToken ct = default)
    {
        var list= await userManager.Users.Select(u => new UserDto() { Id = u.Id, PhoneNumber = u.PhoneNumber, UserNameOrEmail = u.Email! }).ToListAsync(ct);
        return Result<List<UserDto>>.Success(list);
    }

    public async Task<Result<string?>> GetUserNameAsync(string userId, CancellationToken ct = default)
    {
        var user = (await userManager.FindByIdAsync(userId));
        if (user == null)
        {
            return Result<string?>.NotFound("User");
        }
        return Result<string?>.Success(user.UserName!);
    }

    public async Task<Result<bool>> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<bool>.NotFound("User");
        }
        var yes = await userManager.IsInRoleAsync(user, role);
        return Result<bool>.Success(yes);
    }

    public async Task<Result> RemoveFromRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.NotFound("User");
        }
        var result = await userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded
            ? Result.Success(ResultStatus.NoContent)
            : Result.Error($"Không thể xóa role: {role}");
    }

    public async Task<Result> UpdateRoleAsync(string roleId, string newRoleName, CancellationToken ct = default)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role is null)
            return Result.NotFound("Role");
        role.Name = newRoleName;
        var result = await roleManager.UpdateAsync(role);
        return result.Succeeded ? Result.Success(ResultStatus.NoContent) : Result.Error(GetIdentityErrors(result).ToArray());
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return principal;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating token");
            return null;
        }
    }

    public async Task<Result<UserDto?>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        // ✅ Optimized: Minimal DB reads
        // Query 1: Get user for password verification (must use UserManager for security)
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result<UserDto?>.NotFound("User");
        }

        // Query 2: Get full UserDTO with roles (happens only after password verification passes)
        var userDto = await GetUserDtoByEmailUsingQueryAsync(request.Email, ct);
        if (userDto == null)
        {
            return Result<UserDto?>.NotFound("User");
        }
        logger.LogInformation("User authenticated (login) successfully: {UserId}", user.Id);
        return Result<UserDto?>.Success(userDto);
    }

    private async Task<UserDto?> GetUserDtoByEmailUsingQueryAsync(string email, CancellationToken ct)
    {
        var pattern = $"%{email}%";
        var user = await userManager.FindByEmailAsync(pattern);
        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);
            return new UserDto() { Id = user.Id, PhoneNumber = user.PhoneNumber, UserNameOrEmail = user.Email!, Roles = [.. roles] };
        }
        return null;
    }
    private static IEnumerable<string> GetIdentityErrors(IdentityResult identityResult)
       => identityResult.Errors.Select(x => x.Description);
}
