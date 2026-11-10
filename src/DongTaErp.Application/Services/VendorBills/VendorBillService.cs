using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.VendorBills;

public class VendorBillService(
    IUnitOfWork unitOfWork,
    ILogger<VendorBillService> logger,
    IValidator<CreateVendorBillDto> createValidator,
    IValidator<UpdateVendorBillDto> updateValidator) : IVendorBillService
{
    public Task<Result<VendorBillDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.VendorBills, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<VendorBillDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.VendorBills, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateVendorBillDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.VendorBills, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateVendorBillDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.VendorBills, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.VendorBills, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.VendorBills, unitOfWork, id, logger, ct);

    public Task<Result<UpdateVendorBillDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.VendorBills, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<VendorBillDto>>> SearchAsync(SimpleSearch search, bool dueOnly = false, CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.VendorBills.Query();
            if (dueOnly)
            {
                var today = DateTime.Today;
                var dueLimit = today.AddDays(7);
                var open = new[] { VendorBillStatus.Posted, VendorBillStatus.PartiallyPaid };
                query = query.Where(x => open.Contains(x.Status) && x.DueDate != null && x.DueDate <= dueLimit && x.Total > x.PaidAmount);
            }
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Number.ToLower(), term) ||
                    EF.Functions.Like(x.Partner.Name.ToLower(), term) ||
                    (x.SupplierNumber != null && EF.Functions.Like(x.SupplierNumber.ToLower(), term)));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToVendorBillDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<VendorBillDto>>.Success(page);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching vendor bills");
            return Result<PaginatedList<VendorBillDto>>.CriticalError("Internal server error occurred while searching vendor bills.");
        }
    }

    public async Task<Result> PostAsync(Guid id, CancellationToken ct = default)
    {
        var bill = await unitOfWork.VendorBills.Query().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (bill is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(VendorBill), id));
        if (bill.Status != VendorBillStatus.Draft)
            return Result.Error("Chỉ ghi sổ hóa đơn đang nháp.");
        if (bill.Lines.Count == 0)
            return Result.Error("Hóa đơn chưa có dòng.");

        var order = await unitOfWork.PurchaseOrders.GetByIdAsync(bill.PurchaseOrderId, ct);
        if (order is null || order.Status != PurchaseOrderStatus.Confirmed)
            return Result.Error("Đơn mua phải đã xác nhận.");
        if (order.PartnerId != bill.PartnerId)
            return Result.Error("Nhà cung cấp hóa đơn không khớp đơn mua.");

        var lineIds = bill.Lines.Select(x => x.PurchaseOrderLineId).ToList();
        var received = await unitOfWork.GoodsReceiptLines.Query()
            .Where(x => lineIds.Contains(x.PurchaseOrderLineId) && x.GoodsReceipt.Status == GoodsReceiptStatus.Posted)
            .GroupBy(x => x.PurchaseOrderLineId)
            .Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) })
            .ToListAsync(ct);
        var billed = await unitOfWork.VendorBillLines.Query()
            .Where(x => lineIds.Contains(x.PurchaseOrderLineId) && x.VendorBill.Status != VendorBillStatus.Draft && x.VendorBill.Status != VendorBillStatus.Cancelled)
            .GroupBy(x => x.PurchaseOrderLineId)
            .Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Qty) })
            .ToListAsync(ct);

        foreach (var line in bill.Lines)
        {
            var inQty = received.FirstOrDefault(x => x.LineId == line.PurchaseOrderLineId)?.Qty ?? 0;
            var billedQty = billed.FirstOrDefault(x => x.LineId == line.PurchaseOrderLineId)?.Qty ?? 0;
            if (billedQty + line.Qty > inQty)
                return Result.Error("Số lượng hóa đơn vượt số đã nhập kho.");
        }

        bill.Status = VendorBillStatus.Posted;
        bill.PostedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var bill = await unitOfWork.VendorBills.GetByIdAsync(id, ct);
        if (bill is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(VendorBill), id));
        if (bill.PaidAmount > 0)
            return Result.Error("Không hủy hóa đơn đã được thanh toán. Hủy phân bổ trước.");
        if (bill.Status == VendorBillStatus.Cancelled)
            return Result.Error("Hóa đơn đã hủy.");

        bill.Status = VendorBillStatus.Cancelled;
        bill.CancelledAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
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
                LineId = x.Id,
                Label = $"{x.Sku} - {x.Name}",
                OrderedQty = x.Qty,
                DoneQty = done,
                RelatedQty = billedQty,
                RemainingQty = Math.Max(0, done - billedQty),
                UnitPrice = x.UnitPrice
            };
        }).ToList();
        return Result<List<DocumentLineLookupDto>>.Success(lines);
    }

    public async Task<Result<List<OpenDocumentDto>>> GetOpenVendorBillsAsync(Guid partnerId, CancellationToken ct = default)
       => Result<List<OpenDocumentDto>>.Success(await unitOfWork.VendorBills.Query()
           .Where(x => x.PartnerId == partnerId && (x.Status == VendorBillStatus.Posted || x.Status == VendorBillStatus.PartiallyPaid))
           .OrderBy(x => x.BillDate)
           .Select(x => new OpenDocumentDto { Id = x.Id, PartnerId = x.PartnerId, Number = x.Number, Total = x.Total, Balance = x.Total - x.PaidAmount })
           .ToListAsync(ct));



    public async Task<Result<OpenDocumentDto?>> GetVendorBillAsync(Guid id, CancellationToken ct = default)
        => Result<OpenDocumentDto?>.Success(await unitOfWork.VendorBills.Query().Where(x => x.Id == id)
            .Select(x => new OpenDocumentDto { Id = x.Id, PartnerId = x.PartnerId, Number = x.Number, Total = x.Total, Balance = x.Total - x.PaidAmount })
            .FirstOrDefaultAsync(ct));

    private static Expression<Func<VendorBill, VendorBillDto>> ToDto() => Extensions.ExpressionToDto();

    private VendorBill CreateEntity(CreateVendorBillDto dto)
    {
        var order = unitOfWork.PurchaseOrders.Query().First(x => x.Id == dto.PurchaseOrderId);
        var bill = new VendorBill
        {
            Number = dto.Number.Trim(),
            SupplierNumber = dto.SupplierNumber,
            PartnerId = order.PartnerId,
            PurchaseOrderId = dto.PurchaseOrderId,
            BillDate = dto.BillDate,
            DueDate = dto.DueDate,
            TaxRate = dto.TaxRate,
            Notes = dto.Notes,
            Status = VendorBillStatus.Draft
        };

        foreach (var line in dto.Lines)
        {
            var orderLine = unitOfWork.PurchaseOrderLines.Query().First(x => x.Id == line.PurchaseOrderLineId);
            bill.Lines.Add(new VendorBillLine
            {
                PurchaseOrderLineId = line.PurchaseOrderLineId,
                ProductId = orderLine.ProductId,
                Qty = line.Qty,
                UnitPrice = line.UnitPrice,
                LineTotal = line.Qty * line.UnitPrice
            });
        }

        Recalculate(bill);
        return bill;
    }

    private static Expression<Func<VendorBill, UpdateVendorBillDto>> ToUpdateDto() => x => new UpdateVendorBillDto
    {
        Id = x.Id,
        Number = x.Number,
        SupplierNumber = x.SupplierNumber,
        BillDate = x.BillDate,
        DueDate = x.DueDate,
        TaxRate = x.TaxRate,
        Notes = x.Notes
    };

    private static void UpdateEntity(VendorBill entity, UpdateVendorBillDto dto)
    {
        if (entity.Status != VendorBillStatus.Draft)
            throw new InvalidOperationException("Chỉ sửa hóa đơn đang nháp.");

        if (dto.Number.HasValueAndIsDifferentFrom(entity.Number))
            entity.Number = dto.Number!.Trim();
        if (dto.SupplierNumber.IsDifferentFrom(entity.SupplierNumber))
            entity.SupplierNumber = dto.SupplierNumber;
        if (dto.BillDate.HasValueAndIsDifferentFrom(entity.BillDate))
            entity.BillDate = dto.BillDate!.Value;
        if (dto.DueDate.IsDifferentFrom(entity.DueDate))
            entity.DueDate = dto.DueDate;
        if (dto.TaxRate.HasValueAndIsDifferentFrom(entity.TaxRate))
            entity.TaxRate = dto.TaxRate!.Value;
        if (dto.Notes.IsDifferentFrom(entity.Notes))
            entity.Notes = dto.Notes;

        Recalculate(entity);
    }

    private static void Recalculate(VendorBill bill)
    {
        bill.SubTotal = bill.Lines.Sum(x => x.LineTotal);
        bill.TaxAmount = Math.Round(bill.SubTotal * bill.TaxRate, 2);
        bill.Total = bill.SubTotal + bill.TaxAmount;
    }
}
