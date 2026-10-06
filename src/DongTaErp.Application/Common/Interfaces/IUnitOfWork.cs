namespace DongTaErp.Application.Common.Interfaces;

/// <summary>
/// UnitOfWork interface để quản lý repositories và transactions
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<CompanyProfile> CompanyProfiles { get; }
    IRepository<CategoryType> CategoryTypes { get; }
    IRepository<Category> Categories { get; }
    IRepository<Product> Products { get; }
    IRepository<Warehouse> Warehouses { get; }
    IRepository<StockBalance> StockBalances { get; }
    IRepository<Partner> Partners { get; }
    IRepository<SalesOrder> SalesOrders { get; }
    IRepository<SalesOrderLine> SalesOrderLines { get; }
    IRepository<InventoryTxn> InventoryTxns { get; }
    IRepository<Department> Departments { get; }
    IRepository<JobPosition> JobPositions { get; }
    IRepository<Employee> Employees { get; }
    IRepository<LeaveRequest> LeaveRequests { get; }
    IRepository<GoodsIssue> GoodsIssues { get; }
    IRepository<GoodsIssueLine> GoodsIssueLines { get; }

    /// <summary>
    /// Lưu tất cả thay đổi vào database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Bắt đầu một transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Commit transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Rollback transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
