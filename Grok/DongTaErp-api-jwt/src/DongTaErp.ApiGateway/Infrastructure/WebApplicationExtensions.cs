using System.Reflection;

namespace DongTaErp.ApiGateway.Infrastructure;

public static class WebApplicationExtensions
{
    private static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
    {
        var groupName = group.GroupName ?? group.GetType().Name;
        var route = app.MapGroup($"/api/{groupName}").WithTags(groupName);
        return groupName == "Auth" ? route : route.RequireAuthorization();
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app.MapGroup(instance));
            }
        }

        return app;
    }
}
