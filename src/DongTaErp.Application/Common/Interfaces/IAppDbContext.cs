namespace DongTaErp.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<CompanyProfile> CompanyProfiles { get; }
    DbSet<Product> Products { get; }
    DbSet<Partner> Partners { get; }
    DbSet<SalesOrder> SalesOrders { get; }
    DbSet<SalesOrderLine> SalesOrderLines { get; }
    DbSet<InventoryTxn> InventoryTxns { get; }
    DbSet<Department> Departments { get; }
    DbSet<JobPosition> JobPositions { get; }
    DbSet<Employee> Employees { get; }
    DbSet<LeaveRequest> LeaveRequests { get; }
}