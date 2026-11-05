using DongTaErp.ApiGateway.Services;
using DongTaErp.Application.Common.Interfaces;
using DongTaErp.Domain.Entities;
using DongTaErp.Infrastructure.Data;
using DongTaErp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DongTaErp.ApiGateway;

public static class DependencyInjection
{
    public static void AddApiGatewayServices(this IHostApplicationBuilder builder)
    {
        builder.AddApplicationServices();
        builder.AddInfrastructureServices();

        builder.Services.AddScoped<IUser, CurrentUser>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter()
            );
        });
        var key = builder.Configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey not configured");
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
                };
            });

        builder.Services.AddAuthorization();

        builder.Services
            .AddIdentityCore<AppUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        builder.Services.AddAuthorization();

    }
}