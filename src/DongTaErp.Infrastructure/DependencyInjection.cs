using DongTaErp.Infrastructure.Data;
using DongTaErp.Infrastructure.Data.Interceptors;
using DongTaErp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongTaErp.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source = (localdb)\\MSSQLLocalDB;Initial Catalog = ErpLite; Integrated Security = True; Connect Timeout = 30; Encrypt=True;Trust Server Certificate=False;Application Intent = ReadWrite; Multi Subnet Failover=False;Command Timeout = 30";
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddDbContext<AppDbContext>((sp, optrions) =>
        {
            optrions.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            optrions.UseSqlServer(connectionString);
        });
        builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        builder.Services.AddSingleton(TimeProvider.System);

        // Register UnitOfWork and Repositories
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    }
}