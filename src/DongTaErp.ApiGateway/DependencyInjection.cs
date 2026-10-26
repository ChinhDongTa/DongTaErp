using DongTaErp.ApiGateway.Services;
using DongTaErp.Application.Common.Interfaces;
using DongTaErp.Domain.Entities;
using DongTaErp.Infrastructure.Data;
using DongTaErp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

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
        builder.Services.AddAuthentication()
          .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

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