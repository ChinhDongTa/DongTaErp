using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace DongTaErp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IAppDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<CompanyProfile>? _companyProfiles;
    private IRepository<CategoryType>? _categoryTypes;
    private IRepository<Category>? _categories;
    private IRepository<Product>? _products;
    private IRepository<Warehouse>? _warehouses;
    private IRepository<StockBalance>? _stockBalances;
    private IRepository<Partner>? _partners;
    private IRepository<SalesOrder>? _salesOrders;
    private IRepository<SalesOrderLine>? _salesOrderLines;
    private IRepository<InventoryTxn>? _inventoryTxns;
    private IRepository<Department>? _departments;
    private IRepository<JobPosition>? _jobPositions;
    private IRepository<Employee>? _employees;
    private IRepository<LeaveRequest>? _leaveRequests;
    private IRepository<Payroll>? _payrolls;
    private IRepository<GoodsIssue>? _goodsIssues;
    private IRepository<GoodsIssueLine>? _goodsIssueLines;
    private IRepository<Invoice>? _invoices;
    private IRepository<InvoiceLine>? _invoiceLines;
    private IRepository<Payment>? _payments;
    private IRepository<PaymentAllocation>? _paymentAllocations;
    private IRepository<PurchaseOrder>? _purchaseOrders;
    private IRepository<PurchaseOrderLine>? _purchaseOrderLines;
    private IRepository<GoodsReceipt>? _goodsReceipts;
    private IRepository<GoodsReceiptLine>? _goodsReceiptLines;
    private IRepository<VendorBill>? _vendorBills;
    private IRepository<VendorBillLine>? _vendorBillLines;
    private IRepository<Disbursement>? _disbursements;
    private IRepository<DisbursementAllocation>? _disbursementAllocations;
    public UnitOfWork(IAppDbContext context)
    {
        _context = context;
    }

    public IRepository<CompanyProfile> CompanyProfiles =>
        _companyProfiles ??= new GenericRepository<CompanyProfile>(_context);

    public IRepository<CategoryType> CategoryTypes =>
        _categoryTypes ??= new GenericRepository<CategoryType>(_context);

    public IRepository<Category> Categories =>
        _categories ??= new GenericRepository<Category>(_context);

    public IRepository<Product> Products =>
        _products ??= new GenericRepository<Product>(_context);

    public IRepository<Warehouse> Warehouses =>
        _warehouses ??= new GenericRepository<Warehouse>(_context);

    public IRepository<StockBalance> StockBalances =>
        _stockBalances ??= new GenericRepository<StockBalance>(_context);

    public IRepository<Partner> Partners =>
        _partners ??= new GenericRepository<Partner>(_context);

    public IRepository<SalesOrder> SalesOrders =>
        _salesOrders ??= new GenericRepository<SalesOrder>(_context);

    public IRepository<SalesOrderLine> SalesOrderLines =>
        _salesOrderLines ??= new GenericRepository<SalesOrderLine>(_context);

    public IRepository<InventoryTxn> InventoryTxns =>
        _inventoryTxns ??= new GenericRepository<InventoryTxn>(_context);

    public IRepository<Department> Departments =>
        _departments ??= new GenericRepository<Department>(_context);

    public IRepository<JobPosition> JobPositions =>
        _jobPositions ??= new GenericRepository<JobPosition>(_context);

    public IRepository<Employee> Employees =>
        _employees ??= new GenericRepository<Employee>(_context);

    public IRepository<LeaveRequest> LeaveRequests =>
        _leaveRequests ??= new GenericRepository<LeaveRequest>(_context);
    public IRepository<Payroll> Payrolls =>
        _payrolls ??= new GenericRepository<Payroll>(_context);
    public IRepository<GoodsIssue> GoodsIssues =>
        _goodsIssues ??= new GenericRepository<GoodsIssue>(_context);
    public IRepository<GoodsIssueLine> GoodsIssueLines =>
        _goodsIssueLines ??= new GenericRepository<GoodsIssueLine>(_context);
    public IRepository<Invoice> Invoices =>
        _invoices ??= new GenericRepository<Invoice>(_context);
    public IRepository<InvoiceLine> InvoiceLines =>
        _invoiceLines ??= new GenericRepository<InvoiceLine>(_context);
    public IRepository<Payment> Payments =>
        _payments ??= new GenericRepository<Payment>(_context);
    public IRepository<PaymentAllocation> PaymentAllocations =>
        _paymentAllocations ??= new GenericRepository<PaymentAllocation>(_context);
    public IRepository<PurchaseOrder> PurchaseOrders =>
        _purchaseOrders ??= new GenericRepository<PurchaseOrder>(_context);
    public IRepository<PurchaseOrderLine> PurchaseOrderLines =>
        _purchaseOrderLines ??= new GenericRepository<PurchaseOrderLine>(_context);
    public IRepository<GoodsReceipt> GoodsReceipts =>
        _goodsReceipts ??= new GenericRepository<GoodsReceipt>(_context);
    public IRepository<GoodsReceiptLine> GoodsReceiptLines =>
        _goodsReceiptLines ??= new GenericRepository<GoodsReceiptLine>(_context);
    public IRepository<VendorBill> VendorBills =>
        _vendorBills ??= new GenericRepository<VendorBill>(_context);
    public IRepository<VendorBillLine> VendorBillLines =>
        _vendorBillLines ??= new GenericRepository<VendorBillLine>(_context);
    public IRepository<Disbursement> Disbursements =>
        _disbursements ??= new GenericRepository<Disbursement>(_context);
    public IRepository<DisbursementAllocation> DisbursementAllocations =>
        _disbursementAllocations ??= new GenericRepository<DisbursementAllocation>(_context);   
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await ((DbContext)_context).SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await ((DbContext)_context).Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        if (_context is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync();
        }
    }

    public Task<List<T>> QuerySqlAsync<T>(FormattableString sql, CancellationToken ct = default) where T : class
        => Db.Database.SqlQuery<T>(sql).ToListAsync(ct);

    public Task<List<T>> QuerySqlRawAsync<T>(string sql, IEnumerable<SqlArg>? args = null, CancellationToken ct = default) where T : class
        => Db.Database.SqlQueryRaw<T>(sql, ToParameters(args)).ToListAsync(ct);

    public async Task<T?> ExecuteScalarAsync<T>(FormattableString sql, CancellationToken ct = default)
        => await Db.Database.SqlQuery<T>(sql).FirstOrDefaultAsync(ct);

    public Task<int> ExecuteSqlAsync(FormattableString sql, CancellationToken ct = default)
        => Db.Database.ExecuteSqlAsync(sql, ct);

    public Task<List<T>> ExecuteProcedureAsync<T>(string procedureName, IEnumerable<SqlArg>? args = null, CancellationToken ct = default) where T : class
    {
        var name = SafeName(procedureName);
        var parameters = ToParameters(args);
        var call = parameters.Length == 0
            ? $"EXEC {name}"
            : $"EXEC {name} {string.Join(", ", parameters.Select(p => p.ParameterName))}";
        return Db.Database.SqlQueryRaw<T>(call, parameters).ToListAsync(ct);
    }

    public async Task<int> ExecuteProcedureAsync(string procedureName, IEnumerable<SqlArg>? args = null, CancellationToken ct = default)
    {
        var name = SafeName(procedureName);
        await using var command = Db.Database.GetDbConnection().CreateCommand();
        command.CommandText = name;
        command.CommandType = CommandType.StoredProcedure;
        command.Transaction = Db.Database.CurrentTransaction?.GetDbTransaction();
        foreach (var parameter in ToParameters(args))
            command.Parameters.Add(parameter);

        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync(ct);

        var affected = await command.ExecuteNonQueryAsync(ct);
        foreach (SqlParameter parameter in command.Parameters)
        {
            var source = args?.FirstOrDefault(x => Normalize(x.Name) == parameter.ParameterName);
            if (source is { Direction: SqlArgDirection.Output or SqlArgDirection.InputOutput })
                source.SetValue(parameter.Value is DBNull ? null : parameter.Value);
        }
        return affected;
    }

    private DbContext Db => (DbContext)_context;

    private static SqlParameter[] ToParameters(IEnumerable<SqlArg>? args)
        => (args ?? []).Select(x =>
        {
            var parameter = new SqlParameter(Normalize(x.Name), x.Value ?? DBNull.Value)
            {
                Direction = x.Direction switch
                {
                    SqlArgDirection.Output => ParameterDirection.Output,
                    SqlArgDirection.InputOutput => ParameterDirection.InputOutput,
                    _ => ParameterDirection.Input
                }
            };
            if (x.Size is not null)
                parameter.Size = x.Size.Value;
            return parameter;
        }).ToArray();

    private static string Normalize(string name) => name.StartsWith('@') ? name : "@" + name;

    private static string SafeName(string name)
    {
        if (!Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)?$"))
            throw new ArgumentException("Tên function hoặc procedure không hợp lệ.");
        return name;
    }
}
