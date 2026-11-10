using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.PurchaseOrders;

public class PurchaseOrderService(
    IUnitOfWork unitOfWork,
    ILogger<PurchaseOrderService> logger,
    IValidator<CreatePurchaseOrderDto> createValidator,
    IValidator<UpdatePurchaseOrderDto> updateValidator) : IPurchaseOrderService
{
    public Task<Result<PurchaseOrderDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.PurchaseOrders, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<PurchaseOrderDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.PurchaseOrders, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdatePurchaseOrderDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.PurchaseOrders, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.PurchaseOrders, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.PurchaseOrders, unitOfWork, id, logger, ct);

    public Task<Result<UpdatePurchaseOrderDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.PurchaseOrders, ToUpdateDto(), id, logger, ct);


    public async Task<Result<string>> CreateAsync(CreatePurchaseOrderDto dto, CancellationToken ct = default)
    {
        var number = dto.Number.Trim();
        var exists = await unitOfWork.PurchaseOrders.Query().AnyAsync(x => x.Number == number, ct);
        if (exists)
            return Result<string>.Error($"Số đơn {number} đã tồn tại.");
        return await CrudOperations.CreateAsync(unitOfWork.PurchaseOrders, unitOfWork, createValidator, CreateEntity, dto, logger, ct);
    }

    public async Task<Result<PaginatedList<PurchaseOrderDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.PurchaseOrders.Query();
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Number.ToLower(), term) ||
                    EF.Functions.Like(x.Partner.Name.ToLower(), term) ||
                    (x.Notes != null && EF.Functions.Like(x.Notes.ToLower(), term)));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToPurchaseOrderDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<PurchaseOrderDto>>.Success(page);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching purchase orders");
            return Result<PaginatedList<PurchaseOrderDto>>.CriticalError("Internal server error occurred while searching purchase orders.");
        }
    }

    public async Task<Result> ConfirmAsync(Guid id, CancellationToken ct = default)
    {
        var order = await unitOfWork.PurchaseOrders.Query().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (order is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(PurchaseOrder), id));
        if (order.Status != PurchaseOrderStatus.Draft)
            return Result.Error("Chỉ xác nhận đơn đang nháp.");
        if (order.Lines.Count == 0)
            return Result.Error("Đơn mua chưa có dòng hàng.");

        order.Status = PurchaseOrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var order = await unitOfWork.PurchaseOrders.GetByIdAsync(id, ct);
        if (order is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(PurchaseOrder), id));
        if (order.Status == PurchaseOrderStatus.Cancelled)
            return Result.Error("Đơn đã hủy.");

        var hasPostedReceipt = await unitOfWork.GoodsReceipts.Query()
            .AnyAsync(x => x.PurchaseOrderId == id && x.Status == GoodsReceiptStatus.Posted, ct);
        if (hasPostedReceipt)
            return Result.Error("Không hủy đơn đã có phiếu nhập đã ghi sổ. Hủy phiếu nhập trước.");

        order.Status = PurchaseOrderStatus.Cancelled;
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    private static Expression<Func<PurchaseOrder, PurchaseOrderDto>> ToDto() => Extensions.ExpressionToDto();

    private static PurchaseOrder CreateEntity(CreatePurchaseOrderDto dto)
    {
        var order = new PurchaseOrder
        {
            Number = dto.Number.Trim(),
            PartnerId = dto.PartnerId,
            WarehouseId = dto.WarehouseId,
            OrderDate = dto.OrderDate,
            TaxRate = dto.TaxRate,
            Notes = dto.Notes,
            Status = PurchaseOrderStatus.Draft
        };

        foreach (var line in dto.Lines)
        {
            order.Lines.Add(new PurchaseOrderLine
            {
                ProductId = line.ProductId,
                Qty = line.Qty,
                UnitPrice = line.UnitPrice,
                LineTotal = line.Qty * line.UnitPrice
            });
        }

        Recalculate(order);
        return order;
    }

    private static Expression<Func<PurchaseOrder, UpdatePurchaseOrderDto>> ToUpdateDto() => x => new UpdatePurchaseOrderDto
    {
        Id = x.Id,
        Number = x.Number,
        PartnerId = x.PartnerId,
        WarehouseId = x.WarehouseId,
        OrderDate = x.OrderDate,
        TaxRate = x.TaxRate,
        Notes = x.Notes
    };

    private static void UpdateEntity(PurchaseOrder entity, UpdatePurchaseOrderDto dto)
    {
        if (entity.Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Chỉ sửa đơn đang nháp.");

        if (dto.Number.HasValueAndIsDifferentFrom(entity.Number))
            entity.Number = dto.Number!.Trim();
        if (dto.PartnerId.HasValueAndIsDifferentFrom(entity.PartnerId))
            entity.PartnerId = dto.PartnerId!.Value;
        if (dto.WarehouseId.HasValueAndIsDifferentFrom(entity.WarehouseId))
            entity.WarehouseId = dto.WarehouseId!.Value;
        if (dto.OrderDate.HasValueAndIsDifferentFrom(entity.OrderDate))
            entity.OrderDate = dto.OrderDate!.Value;
        if (dto.TaxRate.HasValueAndIsDifferentFrom(entity.TaxRate))
            entity.TaxRate = dto.TaxRate!.Value;
        if (dto.Notes.IsDifferentFrom(entity.Notes))
            entity.Notes = dto.Notes;

        Recalculate(entity);
    }

    private static void Recalculate(PurchaseOrder order)
    {
        order.SubTotal = order.Lines.Sum(x => x.LineTotal);
        order.TaxAmount = Math.Round(order.SubTotal * order.TaxRate, 2);
        order.Total = order.SubTotal + order.TaxAmount;
    }
}
