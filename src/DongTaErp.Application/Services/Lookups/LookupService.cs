using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.Lookups;

public class LookupService(IUnitOfWork unitOfWork) : ILookupService
{
    public async Task<Result<List<LookupItemDto>>> GetWarehousesAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.Warehouses.Query().Where(x => x.IsActive).OrderBy(x => x.Code)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Code + " - " + x.Name }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetProductsAsync(bool salePrice = true, CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.Products.Query().Where(x => x.IsActive).OrderBy(x => x.Sku)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Sku + " - " + x.Name, Price = salePrice ? x.SalePrice : x.CostPrice }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetCustomersAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.Partners.Query()
            .Where(x => x.IsActive && (x.Type == PartnerType.Customer || x.Type == PartnerType.Both)).OrderBy(x => x.Code)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Code + " - " + x.Name }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetSuppliersAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.Partners.Query()
            .Where(x => x.IsActive && (x.Type == PartnerType.Supplier || x.Type == PartnerType.Both)).OrderBy(x => x.Code)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Code + " - " + x.Name }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetCategoriesAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.Categories.Query().OrderBy(x => x.Name)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Name }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetDepartmentsAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.Departments.Query().OrderBy(x => x.Code)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Code + " - " + x.Name }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetJobPositionsAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.JobPositions.Query().OrderBy(x => x.Code)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Code + " - " + x.Name }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetEmployeesAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.Employees.Query().OrderBy(x => x.Code)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Code + " - " + x.FullName }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetConfirmedSalesOrdersAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.SalesOrders.Query().Where(x => x.Status == SalesOrderStatus.Confirmed)
            .OrderByDescending(x => x.OrderDate)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Number + " - " + x.Partner.Name, ExtraId = x.WarehouseId }).ToListAsync(ct));

    public async Task<Result<List<LookupItemDto>>> GetConfirmedPurchaseOrdersAsync(CancellationToken ct = default)
        => Result<List<LookupItemDto>>.Success(await unitOfWork.PurchaseOrders.Query().Where(x => x.Status == PurchaseOrderStatus.Confirmed)
            .OrderByDescending(x => x.OrderDate)
            .Select(x => new LookupItemDto { Id = x.Id, Label = x.Number + " - " + x.Partner.Name, ExtraId = x.WarehouseId }).ToListAsync(ct));

   
}
