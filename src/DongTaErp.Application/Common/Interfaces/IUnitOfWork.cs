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
    IRepository<Payroll> Payrolls { get; }
    IRepository<PayrollLine> PayrollLines { get; }
    IRepository<GoodsIssue> GoodsIssues { get; }
    IRepository<GoodsIssueLine> GoodsIssueLines { get; }
    IRepository<Invoice> Invoices { get; }
    IRepository<InvoiceLine> InvoiceLines { get; }
    IRepository<Payment> Payments { get; }
    IRepository<PaymentAllocation> PaymentAllocations { get; }
    IRepository<PurchaseOrder> PurchaseOrders { get; }
    IRepository<PurchaseOrderLine> PurchaseOrderLines { get; }
    IRepository<GoodsReceipt> GoodsReceipts { get; }
    IRepository<GoodsReceiptLine> GoodsReceiptLines { get; }
    IRepository<VendorBill> VendorBills { get; }
    IRepository<VendorBillLine> VendorBillLines { get; }
    IRepository<Disbursement> Disbursements { get; }
    IRepository<DisbursementAllocation> DisbursementAllocations { get; }
    IRepository<SalaryComponent> SalaryComponents { get; }
    IRepository<EmployeeCompensation> EmployeeCompensations { get; }
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

    /// <summary>Chạy SELECT hoặc function trả nhiều dòng. Dùng nội suy để EF tạo parameter.</summary>
    Task<List<T>> QuerySqlAsync<T>(FormattableString sql, CancellationToken ct = default) where T : class;

    /// <summary>Chạy SQL trả nhiều dòng với parameter thô.</summary>
    Task<List<T>> QuerySqlRawAsync<T>(string sql, IEnumerable<SqlArg>? args = null, CancellationToken ct = default) where T : class;

    /// <summary>Chạy function hoặc SELECT trả một giá trị.</summary>
    Task<T?> ExecuteScalarAsync<T>(FormattableString sql, CancellationToken ct = default);

    /// <summary>Chạy lệnh không trả dòng: UPDATE, hoặc procedure không có result set.</summary>
    Task<int> ExecuteSqlAsync(FormattableString sql, CancellationToken ct = default);

    /// <summary>Gọi stored procedure trả nhiều dòng.</summary>
    Task<List<T>> ExecuteProcedureAsync<T>(string procedureName, IEnumerable<SqlArg>? args = null, CancellationToken ct = default) where T : class;

    /// <summary>Gọi stored procedure không trả dòng. Hỗ trợ output parameter.</summary>
    Task<int> ExecuteProcedureAsync(string procedureName, IEnumerable<SqlArg>? args = null, CancellationToken ct = default);
}
