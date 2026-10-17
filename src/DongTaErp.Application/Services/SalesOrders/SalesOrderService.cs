using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.SalesOrders;

public class SalesOrderService
    : Base.GenericCrudService<SalesOrder, SalesOrderDto, CreateSalesOrderDto, UpdateSalesOrderDto>,
      ISalesOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SalesOrderService> _logger;

    public SalesOrderService(
        IUnitOfWork unitOfWork,
        ILogger<SalesOrderService> logger,
        IValidator<CreateSalesOrderDto> createValidator,
        IValidator<UpdateSalesOrderDto> updateValidator)
        : base(unitOfWork.SalesOrders, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public override async Task<Result<string>> CreateAsync(CreateSalesOrderDto dto, CancellationToken ct = default)
    {
        var number = dto.Number.Trim();
        var exists = await _unitOfWork.SalesOrders.Query().AnyAsync(x => x.Number == number, ct);
        if (exists)
            return Result<string>.Error($"Số đơn {number} đã tồn tại.");
        return await base.CreateAsync(dto, ct);
    }

    public async Task<Result<PaginatedList<SalesOrderDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.SalesOrders.Query();
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
            _logger.LogError(ex, "Error searching sales orders");
            return Result<PaginatedList<SalesOrderDto>>.CriticalError("Internal server error occurred while searching sales orders.");
        }
    }

    public async Task<Result> ConfirmAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _unitOfWork.SalesOrders.Query().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (order is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(SalesOrder), id));
        if (order.Status != SalesOrderStatus.Draft)
            return Result.Error("Chỉ xác nhận đơn đang nháp.");
        if (order.Lines.Count == 0)
            return Result.Error("Đơn bán chưa có dòng hàng.");

        order.Status = SalesOrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _unitOfWork.SalesOrders.GetByIdAsync(id, ct);
        if (order is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(SalesOrder), id));
        if (order.Status == SalesOrderStatus.Cancelled)
            return Result.Error("Đơn đã hủy.");

        var hasPostedIssue = await _unitOfWork.GoodsIssues.Query()
            .AnyAsync(x => x.SalesOrderId == id && x.Status == GoodsIssueStatus.Posted, ct);
        if (hasPostedIssue)
            return Result.Error("Không hủy đơn đã có phiếu xuất đã ghi sổ. Hủy phiếu xuất trước.");

        order.Status = SalesOrderStatus.Cancelled;
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    protected override Expression<Func<SalesOrder, SalesOrderDto>> ToDto() => Extensions.ExpressionToDto();

    protected override SalesOrder CreateEntity(CreateSalesOrderDto dto)
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

    protected override Expression<Func<SalesOrder, UpdateSalesOrderDto>> ToUpdateDto() => x => new UpdateSalesOrderDto
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

    protected override void UpdateEntity(SalesOrder entity, UpdateSalesOrderDto dto)
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
