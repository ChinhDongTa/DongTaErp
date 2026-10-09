using DongTaErp.ApiGateway.Services;
using DongTaErp.Application.Common.Interfaces;

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
    }
}