using DongTaErp.Application.Common.DTOs;
using DongTaErp.Application.Common.Models;

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
    Task<Result<List<DocumentLineLookupDto>>> GetGoodsIssueLinesAsync(Guid salesOrderId, Guid? warehouseId = null, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetGoodsReceiptLinesAsync(Guid purchaseOrderId, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetInvoiceLinesAsync(Guid salesOrderId, CancellationToken ct = default);
    Task<Result<List<DocumentLineLookupDto>>> GetVendorBillLinesAsync(Guid purchaseOrderId, CancellationToken ct = default);
    Task<Result<List<OpenDocumentDto>>> GetOpenInvoicesAsync(Guid partnerId, CancellationToken ct = default);
    Task<Result<List<OpenDocumentDto>>> GetOpenVendorBillsAsync(Guid partnerId, CancellationToken ct = default);
    Task<Result<OpenDocumentDto?>> GetInvoiceAsync(Guid id, CancellationToken ct = default);
    Task<Result<OpenDocumentDto?>> GetVendorBillAsync(Guid id, CancellationToken ct = default);
    Task<Result<List<StockBalanceDto>>> GetStockAsync(Guid? warehouseId = null, bool lowOnly = false, Guid? productId = null, CancellationToken ct = default);
}
