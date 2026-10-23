namespace DongTaErp.Application.Services.Lookups;

public interface ILookupService
{
    Task<Result<List<LookupItemDto>>> GetWarehousesAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetProductsAsync(bool salePrice = true, CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetCustomersAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetSuppliersAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetCategoriesAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetDepartmentsAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetJobPositionsAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetEmployeesAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetConfirmedSalesOrdersAsync(CancellationToken ct = default);
    Task<Result<List<LookupItemDto>>> GetConfirmedPurchaseOrdersAsync(CancellationToken ct = default);
}