using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.SalesOrders;

public class SalesOrderService(
    IUnitOfWork unitOfWork,
    ILogger<SalesOrderService> logger,
    IValidator<CreateSalesOrderDto> createValidator,
    IValidator<UpdateSalesOrderDto> updateValidator) : ISalesOrderService
{
    public Task<Result<SalesOrderDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.SalesOrders, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<SalesOrderDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.SalesOrders, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateSalesOrderDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.SalesOrders, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.SalesOrders, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.SalesOrders, unitOfWork, id, logger, ct);

    public Task<Result<UpdateSalesOrderDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.SalesOrders, ToUpdateDto(), id, logger, ct);


    public async Task<Result<string>> CreateAsync(CreateSalesOrderDto dto, CancellationToken ct = default)
    {
        var number = dto.Number.Trim();
        var exists = await unitOfWork.SalesOrders.Query().AnyAsync(x => x.Number == number, ct);
        if (exists)
            return Result<string>.Error($"Số đơn {number} đã tồn tại.");
        return await CrudOperations.CreateAsync(unitOfWork.SalesOrders, unitOfWork, createValidator, CreateEntity, dto, logger, ct);
    }

    public async Task<Result<PaginatedList<SalesOrderDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.SalesOrders.Query();
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

            var page = await query.ToSalesOrderDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<SalesOrderDto>>.Success(page);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching sales orders");
            return Result<PaginatedList<SalesOrderDto>>.CriticalError("Internal server error occurred while searching sales orders.");
        }
    }

    public async Task<Result<List<NewOrderNoticeDto>>> GetNewOrdersAsync(CancellationToken ct = default)
    {
        var items = await unitOfWork.SalesOrders.Query()
            .Where(x => x.Status == SalesOrderStatus.Draft)
            .OrderByDescending(x => x.Created)
            .Take(20)
            .Select(x => new NewOrderNoticeDto
            {
                Id = x.Id,
                Number = x.Number,
                PartnerName = x.Partner.Name,
                OrderDate = x.OrderDate,
                Total = x.Total
            })
            .ToListAsync(ct);
        return Result<List<NewOrderNoticeDto>>.Success(items);
    }

    public async Task<Result> ConfirmAsync(Guid id, CancellationToken ct = default)
    {
        var order = await unitOfWork.SalesOrders.Query().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (order is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(SalesOrder), id));
        if (order.Status != SalesOrderStatus.Draft)
            return Result.Error("Chỉ xác nhận đơn đang nháp.");
        if (order.Lines.Count == 0)
            return Result.Error("Đơn bán chưa có dòng hàng.");

        order.Status = SalesOrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var order = await unitOfWork.SalesOrders.GetByIdAsync(id, ct);
        if (order is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(SalesOrder), id));
        if (order.Status == SalesOrderStatus.Cancelled)
            return Result.Error("Đơn đã hủy.");

        var hasPostedIssue = await unitOfWork.GoodsIssues.Query()
            .AnyAsync(x => x.SalesOrderId == id && x.Status == GoodsIssueStatus.Posted, ct);
        if (hasPostedIssue)
            return Result.Error("Không hủy đơn đã có phiếu xuất đã ghi sổ. Hủy phiếu xuất trước.");

        order.Status = SalesOrderStatus.Cancelled;
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    private static Expression<Func<SalesOrder, SalesOrderDto>> ToDto() => Extensions.ExpressionToDto();

    private static SalesOrder CreateEntity(CreateSalesOrderDto dto)
    {
        var order = new SalesOrder
        {
            Number = dto.Number.Trim(),
            PartnerId = dto.PartnerId,
            WarehouseId = dto.WarehouseId,
            OrderDate = dto.OrderDate,
            TaxRate = dto.TaxRate,
            Notes = dto.Notes,
            Status = SalesOrderStatus.Draft
        };

        foreach (var line in dto.Lines)
        {
            order.Lines.Add(new SalesOrderLine
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

    private static Expression<Func<SalesOrder, UpdateSalesOrderDto>> ToUpdateDto() => x => new UpdateSalesOrderDto
    {
        Id = x.Id,
        Number = x.Number,
        PartnerId = x.PartnerId,
        WarehouseId = x.WarehouseId,
        OrderDate = x.OrderDate,
        Status = x.Status,
        TaxRate = x.TaxRate,
        Notes = x.Notes,
        ConfirmedAt = x.ConfirmedAt
    };

    private static void UpdateEntity(SalesOrder entity, UpdateSalesOrderDto dto)
    {
        if (entity.Status != SalesOrderStatus.Draft)
            return;

        if (!string.IsNullOrWhiteSpace(dto.Number))
            entity.Number = dto.Number.Trim();
        if (dto.PartnerId.HasValue)
            entity.PartnerId = dto.PartnerId.Value;
        if (dto.WarehouseId.HasValue)
            entity.WarehouseId = dto.WarehouseId.Value;
        if (dto.OrderDate.HasValue)
            entity.OrderDate = dto.OrderDate.Value;
        if (dto.TaxRate.HasValue)
            entity.TaxRate = dto.TaxRate.Value;
        entity.Notes = dto.Notes;
        Recalculate(entity);
    }

    private static void Recalculate(SalesOrder order)
    {
        order.SubTotal = order.Lines.Sum(x => x.LineTotal);
        order.TaxAmount = Math.Round(order.SubTotal * order.TaxRate, 2);
        order.Total = order.SubTotal + order.TaxAmount;
    }
}
