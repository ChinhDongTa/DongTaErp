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

    public async Task<Result<List<DocumentLineLookupDto>>> GetGoodsIssueLinesAsync(Guid salesOrderId, Guid? warehouseId = null, CancellationToken ct = default)
    {
        var orderLines = await unitOfWork.SalesOrderLines.Query().Where(x => x.SalesOrderId == salesOrderId)
            .Select(x => new { x.Id, x.ProductId, x.Product.Sku, x.Product.Name, x.Qty }).ToListAsync(ct);
        var ids = orderLines.Select(x => x.Id).ToList();
        var issued = await unitOfWork.GoodsIssueLines.Query()
            .Where(x => ids.Contains(x.SalesOrderLineId) && x.GoodsIssue.Status == GoodsIssueStatus.Posted)
            .GroupBy(x => x.SalesOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) }).ToListAsync(ct);
        var stocks = warehouseId is null ? [] : await unitOfWork.StockBalances.Query()
            .Where(x => x.WarehouseId == warehouseId && orderLines.Select(l => l.ProductId).Contains(x.ProductId))
            .Select(x => new { x.ProductId, x.Qty }).ToListAsync(ct);
        var lines = orderLines.Select(x =>
        {
            var done = issued.FirstOrDefault(r => r.LineId == x.Id)?.Qty ?? 0;
            return new DocumentLineLookupDto
            {
                LineId = x.Id, ProductId = x.ProductId, Label = $"{x.Sku} - {x.Name}",
                OrderedQty = x.Qty, DoneQty = done, RemainingQty = Math.Max(0, x.Qty - done),
                StockQty = stocks.FirstOrDefault(s => s.ProductId == x.ProductId)?.Qty ?? 0
            };
        }).ToList();
        return Result<List<DocumentLineLookupDto>>.Success(lines);
    }

    public async Task<Result<List<DocumentLineLookupDto>>> GetGoodsReceiptLinesAsync(Guid purchaseOrderId, CancellationToken ct = default)
    {
        var orderLines = await unitOfWork.PurchaseOrderLines.Query().Where(x => x.PurchaseOrderId == purchaseOrderId)
            .Select(x => new { x.Id, x.ProductId, x.Product.Sku, x.Product.Name, x.Qty, x.UnitPrice }).ToListAsync(ct);
        var ids = orderLines.Select(x => x.Id).ToList();
        var received = await unitOfWork.GoodsReceiptLines.Query()
            .Where(x => ids.Contains(x.PurchaseOrderLineId) && x.GoodsReceipt.Status == GoodsReceiptStatus.Posted)
            .GroupBy(x => x.PurchaseOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) }).ToListAsync(ct);
        var lines = orderLines.Select(x =>
        {
            var done = received.FirstOrDefault(r => r.LineId == x.Id)?.Qty ?? 0;
            return new DocumentLineLookupDto
            {
                LineId = x.Id, ProductId = x.ProductId, Label = $"{x.Sku} - {x.Name}",
                OrderedQty = x.Qty, DoneQty = done, RemainingQty = Math.Max(0, x.Qty - done), UnitPrice = x.UnitPrice
            };
        }).ToList();
        return Result<List<DocumentLineLookupDto>>.Success(lines);
    }

    public async Task<Result<List<DocumentLineLookupDto>>> GetInvoiceLinesAsync(Guid salesOrderId, CancellationToken ct = default)
    {
        var orderLines = await unitOfWork.SalesOrderLines.Query().Where(x => x.SalesOrderId == salesOrderId)
            .Select(x => new { x.Id, x.Product.Sku, x.Product.Name, x.Qty, x.UnitPrice }).ToListAsync(ct);
        var ids = orderLines.Select(x => x.Id).ToList();
        var shipped = await unitOfWork.GoodsIssueLines.Query()
            .Where(x => ids.Contains(x.SalesOrderLineId) && x.GoodsIssue.Status == GoodsIssueStatus.Posted)
            .GroupBy(x => x.SalesOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) }).ToListAsync(ct);
        var invoiced = await unitOfWork.InvoiceLines.Query()
            .Where(x => ids.Contains(x.SalesOrderLineId) && x.Invoice.Status != InvoiceStatus.Draft && x.Invoice.Status != InvoiceStatus.Cancelled)
            .GroupBy(x => x.SalesOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Qty) }).ToListAsync(ct);
        var lines = orderLines.Select(x =>
        {
            var done = shipped.FirstOrDefault(s => s.LineId == x.Id)?.Qty ?? 0;
            var billed = invoiced.FirstOrDefault(s => s.LineId == x.Id)?.Qty ?? 0;
            return new DocumentLineLookupDto
            {
                LineId = x.Id, Label = $"{x.Sku} - {x.Name}", OrderedQty = x.Qty,
                DoneQty = done, RelatedQty = billed, RemainingQty = Math.Max(0, done - billed), UnitPrice = x.UnitPrice
            };
        }).ToList();
        return Result<List<DocumentLineLookupDto>>.Success(lines);
    }

    public async Task<Result<List<DocumentLineLookupDto>>> GetVendorBillLinesAsync(Guid purchaseOrderId, CancellationToken ct = default)
    {
        var orderLines = await unitOfWork.PurchaseOrderLines.Query().Where(x => x.PurchaseOrderId == purchaseOrderId)
            .Select(x => new { x.Id, x.Product.Sku, x.Product.Name, x.Qty, x.UnitPrice }).ToListAsync(ct);
        var ids = orderLines.Select(x => x.Id).ToList();
        var received = await unitOfWork.GoodsReceiptLines.Query()
            .Where(x => ids.Contains(x.PurchaseOrderLineId) && x.GoodsReceipt.Status == GoodsReceiptStatus.Posted)
            .GroupBy(x => x.PurchaseOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) }).ToListAsync(ct);
        var billed = await unitOfWork.VendorBillLines.Query()
            .Where(x => ids.Contains(x.PurchaseOrderLineId) && x.VendorBill.Status != VendorBillStatus.Draft && x.VendorBill.Status != VendorBillStatus.Cancelled)
            .GroupBy(x => x.PurchaseOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Qty) }).ToListAsync(ct);
        var lines = orderLines.Select(x =>
        {
            var done = received.FirstOrDefault(s => s.LineId == x.Id)?.Qty ?? 0;
            var billedQty = billed.FirstOrDefault(s => s.LineId == x.Id)?.Qty ?? 0;
            return new DocumentLineLookupDto
            {
                LineId = x.Id, Label = $"{x.Sku} - {x.Name}", OrderedQty = x.Qty,
                DoneQty = done, RelatedQty = billedQty, RemainingQty = Math.Max(0, done - billedQty), UnitPrice = x.UnitPrice
            };
        }).ToList();
        return Result<List<DocumentLineLookupDto>>.Success(lines);
    }

    public async Task<Result<List<OpenDocumentDto>>> GetOpenInvoicesAsync(Guid partnerId, CancellationToken ct = default)
        => Result<List<OpenDocumentDto>>.Success(await unitOfWork.Invoices.Query()
            .Where(x => x.PartnerId == partnerId && (x.Status == InvoiceStatus.Issued || x.Status == InvoiceStatus.PartiallyPaid))
            .OrderBy(x => x.IssueDate)
            .Select(x => new OpenDocumentDto { Id = x.Id, PartnerId = x.PartnerId, Number = x.Number, Total = x.Total, Balance = x.Total - x.PaidAmount })
            .ToListAsync(ct));

    public async Task<Result<List<OpenDocumentDto>>> GetOpenVendorBillsAsync(Guid partnerId, CancellationToken ct = default)
        => Result<List<OpenDocumentDto>>.Success(await unitOfWork.VendorBills.Query()
            .Where(x => x.PartnerId == partnerId && (x.Status == VendorBillStatus.Posted || x.Status == VendorBillStatus.PartiallyPaid))
            .OrderBy(x => x.BillDate)
            .Select(x => new OpenDocumentDto { Id = x.Id, PartnerId = x.PartnerId, Number = x.Number, Total = x.Total, Balance = x.Total - x.PaidAmount })
            .ToListAsync(ct));

    public async Task<Result<OpenDocumentDto?>> GetInvoiceAsync(Guid id, CancellationToken ct = default)
        => Result<OpenDocumentDto?>.Success(await unitOfWork.Invoices.Query().Where(x => x.Id == id)
            .Select(x => new OpenDocumentDto { Id = x.Id, PartnerId = x.PartnerId, Number = x.Number, Total = x.Total, Balance = x.Total - x.PaidAmount })
            .FirstOrDefaultAsync(ct));

    public async Task<Result<OpenDocumentDto?>> GetVendorBillAsync(Guid id, CancellationToken ct = default)
        => Result<OpenDocumentDto?>.Success(await unitOfWork.VendorBills.Query().Where(x => x.Id == id)
            .Select(x => new OpenDocumentDto { Id = x.Id, PartnerId = x.PartnerId, Number = x.Number, Total = x.Total, Balance = x.Total - x.PaidAmount })
            .FirstOrDefaultAsync(ct));

    public async Task<Result<List<StockBalanceDto>>> GetStockAsync(Guid? warehouseId = null, bool lowOnly = false, Guid? productId = null, CancellationToken ct = default)
    {
        var query = unitOfWork.StockBalances.Query();
        if (warehouseId is not null)
            query = query.Where(x => x.WarehouseId == warehouseId);
        if (productId is not null)
            query = query.Where(x => x.ProductId == productId);
        if (lowOnly)
            query = query.Where(x => x.Qty <= x.MinQty);
        var rows = await query.OrderBy(x => x.Warehouse.Code).ThenBy(x => x.Product.Sku)
            .Select(x => new StockBalanceDto
            {
                Id = x.Id, ProductSku = x.Product.Sku, ProductName = x.Product.Name,
                WarehouseName = x.Warehouse.Name, Qty = x.Qty, MinQty = x.MinQty, CreatedAt = x.Created
            }).ToListAsync(ct);
        return Result<List<StockBalanceDto>>.Success(rows);
    }
}
