using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.GoodsReceipts;

/// <summary>
/// Phiếu nhập. Post mới cộng StockBalance và ghi InventoryTxn.
/// Hủy phiếu đã post thì đảo tồn. Workflow post/hủy có thể tách MediatR sau.
/// </summary>
public class GoodsReceiptService
    : Base.GenericCrudService<GoodsReceipt, GoodsReceiptDto, CreateGoodsReceiptDto, UpdateGoodsReceiptDto>,
      IGoodsReceiptService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GoodsReceiptService> _logger;

    public GoodsReceiptService(
        IUnitOfWork unitOfWork,
        ILogger<GoodsReceiptService> logger,
        IValidator<CreateGoodsReceiptDto> createValidator,
        IValidator<UpdateGoodsReceiptDto> updateValidator)
        : base(unitOfWork.GoodsReceipts, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<GoodsReceiptDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.GoodsReceipts.Query();
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Code.ToLower(), term) ||
                    EF.Functions.Like(x.PurchaseOrder.Number.ToLower(), term) ||
                    EF.Functions.Like(x.Warehouse.Name.ToLower(), term) ||
                    (x.SupplierRef != null && EF.Functions.Like(x.SupplierRef.ToLower(), term)));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToGoodsReceiptDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<GoodsReceiptDto>>.Success(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching goods receipts");
            return Result<PaginatedList<GoodsReceiptDto>>.CriticalError("Internal server error occurred while searching goods receipts.");
        }
    }

    public async Task<Result> PostAsync(Guid id, CancellationToken ct = default)
    {
        var receipt = await _unitOfWork.GoodsReceipts.Query()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (receipt is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(GoodsReceipt), id));
        if (receipt.Status != GoodsReceiptStatus.Draft)
            return Result.Error("Chỉ ghi sổ phiếu đang nháp.");
        if (receipt.Lines.Count == 0)
            return Result.Error("Phiếu nhập chưa có dòng.");

        var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(receipt.PurchaseOrderId, ct);
        if (order is null || order.Status != PurchaseOrderStatus.Confirmed)
            return Result.Error("Đơn mua phải đã xác nhận.");

        var lineIds = receipt.Lines.Select(x => x.PurchaseOrderLineId).ToList();
        var orderLines = await _unitOfWork.PurchaseOrderLines.Query()
            .Where(x => lineIds.Contains(x.Id) && x.PurchaseOrderId == receipt.PurchaseOrderId)
            .ToListAsync(ct);

        var received = await _unitOfWork.GoodsReceiptLines.Query()
            .Where(x => lineIds.Contains(x.PurchaseOrderLineId) && x.GoodsReceipt.Status == GoodsReceiptStatus.Posted)
            .GroupBy(x => x.PurchaseOrderLineId)
            .Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) })
            .ToListAsync(ct);

        foreach (var line in receipt.Lines)
        {
            var orderLine = orderLines.FirstOrDefault(x => x.Id == line.PurchaseOrderLineId);
            if (orderLine is null)
                return Result.Error("Dòng phiếu không thuộc đơn mua.");
            if (orderLine.ProductId != line.ProductId)
                return Result.Error("Sản phẩm dòng phiếu không khớp dòng đơn.");

            var already = received.FirstOrDefault(x => x.LineId == line.PurchaseOrderLineId)?.Qty ?? 0;
            if (already + line.Quantity > orderLine.Qty)
                return Result.Error("Số nhập vượt số còn lại của dòng đơn.");
        }

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            foreach (var line in receipt.Lines)
            {
                var balance = await _unitOfWork.StockBalances.Query()
                    .FirstOrDefaultAsync(x => x.ProductId == line.ProductId && x.WarehouseId == receipt.WarehouseId, ct);
                if (balance is null)
                {
                    await _unitOfWork.StockBalances.AddAsync(new StockBalance
                    {
                        ProductId = line.ProductId,
                        WarehouseId = receipt.WarehouseId,
                        Qty = line.Quantity
                    }, ct);
                }
                else
                {
                    balance.Qty += line.Quantity;
                }

                await _unitOfWork.InventoryTxns.AddAsync(new InventoryTxn
                {
                    ProductId = line.ProductId,
                    WarehouseId = receipt.WarehouseId,
                    Type = InventoryTxnType.In,
                    QtyChange = line.Quantity,
                    Reference = receipt.Code,
                    Note = "Nhập kho mua hàng"
                }, ct);
            }

            receipt.Status = GoodsReceiptStatus.Posted;
            receipt.PostedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            _logger.LogError(ex, "Error posting goods receipt {Id}", id);
            return Result.Error("Không ghi sổ được phiếu nhập.");
        }
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var receipt = await _unitOfWork.GoodsReceipts.Query()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (receipt is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(GoodsReceipt), id));
        if (receipt.Status == GoodsReceiptStatus.Cancelled)
            return Result.Error("Phiếu đã hủy.");

        if (receipt.Status == GoodsReceiptStatus.Draft)
        {
            receipt.Status = GoodsReceiptStatus.Cancelled;
            receipt.CancelledAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            foreach (var line in receipt.Lines)
            {
                var balance = await _unitOfWork.StockBalances.Query()
                    .FirstOrDefaultAsync(x => x.ProductId == line.ProductId && x.WarehouseId == receipt.WarehouseId, ct);
                if (balance is null || balance.Qty < line.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync(ct);
                    return Result.Error("Không đủ tồn để hoàn phiếu nhập.");
                }

                balance.Qty -= line.Quantity;
                await _unitOfWork.InventoryTxns.AddAsync(new InventoryTxn
                {
                    ProductId = line.ProductId,
                    WarehouseId = receipt.WarehouseId,
                    Type = InventoryTxnType.In,
                    QtyChange = -line.Quantity,
                    Reference = receipt.Code,
                    Note = "Hủy phiếu nhập"
                }, ct);
            }

            receipt.Status = GoodsReceiptStatus.Cancelled;
            receipt.CancelledAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            _logger.LogError(ex, "Error cancelling goods receipt {Id}", id);
            return Result.Error("Không hủy được phiếu nhập.");
        }
    }

    public async Task<Result<List<DocumentLineLookupDto>>> GetGoodsReceiptLinesAsync(Guid purchaseOrderId, CancellationToken ct = default)
    {
        var orderLines = await _unitOfWork.PurchaseOrderLines.Query().Where(x => x.PurchaseOrderId == purchaseOrderId)
            .Select(x => new { x.Id, x.ProductId, x.Product.Sku, x.Product.Name, x.Qty, x.UnitPrice }).ToListAsync(ct);
        var ids = orderLines.Select(x => x.Id).ToList();
        var received = await _unitOfWork.GoodsReceiptLines.Query()
            .Where(x => ids.Contains(x.PurchaseOrderLineId) && x.GoodsReceipt.Status == GoodsReceiptStatus.Posted)
            .GroupBy(x => x.PurchaseOrderLineId).Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) }).ToListAsync(ct);
        var lines = orderLines.Select(x =>
        {
            var done = received.FirstOrDefault(r => r.LineId == x.Id)?.Qty ?? 0;
            return new DocumentLineLookupDto
            {
                LineId = x.Id,
                ProductId = x.ProductId,
                Label = $"{x.Sku} - {x.Name}",
                OrderedQty = x.Qty,
                DoneQty = done,
                RemainingQty = Math.Max(0, x.Qty - done),
                UnitPrice = x.UnitPrice
            };
        }).ToList();
        return Result<List<DocumentLineLookupDto>>.Success(lines);
    }

    protected override Expression<Func<GoodsReceipt, GoodsReceiptDto>> ToDto() => Extensions.ExpressionToDto();

    protected override GoodsReceipt CreateEntity(CreateGoodsReceiptDto dto)
    {
        var receipt = new GoodsReceipt
        {
            Code = dto.Code.Trim(),
            PurchaseOrderId = dto.PurchaseOrderId,
            WarehouseId = dto.WarehouseId,
            ReceiptDate = dto.ReceiptDate,
            SupplierRef = dto.SupplierRef,
            Notes = dto.Notes,
            Status = GoodsReceiptStatus.Draft
        };

        foreach (var line in dto.Lines)
        {
            var orderLine = _unitOfWork.PurchaseOrderLines.Query().First(x => x.Id == line.PurchaseOrderLineId);
            receipt.Lines.Add(new GoodsReceiptLine
            {
                PurchaseOrderLineId = line.PurchaseOrderLineId,
                ProductId = orderLine.ProductId,
                Quantity = line.Quantity,
                Note = line.Note
            });
        }

        return receipt;
    }

    protected override Expression<Func<GoodsReceipt, UpdateGoodsReceiptDto>> ToUpdateDto() => x => new UpdateGoodsReceiptDto
    {
        Id = x.Id,
        Code = x.Code,
        WarehouseId = x.WarehouseId,
        ReceiptDate = x.ReceiptDate,
        SupplierRef = x.SupplierRef,
        Notes = x.Notes
    };

    protected override void UpdateEntity(GoodsReceipt entity, UpdateGoodsReceiptDto dto)
    {
        if (entity.Status != GoodsReceiptStatus.Draft)
            throw new InvalidOperationException("Chỉ sửa phiếu đang nháp.");

        if (dto.Code.HasValueAndIsDifferentFrom(entity.Code))
            entity.Code = dto.Code!.Trim();
        if (dto.WarehouseId.HasValueAndIsDifferentFrom(entity.WarehouseId))
            entity.WarehouseId = dto.WarehouseId!.Value;
        if (dto.ReceiptDate.HasValueAndIsDifferentFrom(entity.ReceiptDate))
            entity.ReceiptDate = dto.ReceiptDate!.Value;
        if (dto.SupplierRef.IsDifferentFrom(entity.SupplierRef))
            entity.SupplierRef = dto.SupplierRef;
        if (dto.Notes.IsDifferentFrom(entity.Notes))
            entity.Notes = dto.Notes;
    }
}
