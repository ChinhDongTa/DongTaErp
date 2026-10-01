using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.Metrics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace DongTaErp.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();
        var cs = Environment.GetEnvironmentVariable("ERP_CS")
                 ?? "Data Source = (localdb)\\MSSQLLocalDB;Initial Catalog = ErpLite; Integrated Security = True; Connect Timeout = 30; Encrypt=True;Trust Server Certificate=False;Application Intent = ReadWrite; Multi Subnet Failover=False;Command Timeout = 30";
        options.UseSqlServer(cs);
        return new AppDbContext(options.Options);
    }
}
