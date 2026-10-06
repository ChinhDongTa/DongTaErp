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
    private IRepository<GoodsIssue>? _goodsIssues;
    private IRepository<GoodsIssueLine>? _goodsIssueLines;
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
    public IRepository<GoodsIssue> GoodsIssues =>
        _goodsIssues ??= new GenericRepository<GoodsIssue>(_context);
    public IRepository<GoodsIssueLine> GoodsIssueLines =>
        _goodsIssueLines ??= new GenericRepository<GoodsIssueLine>(_context);   
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
}
