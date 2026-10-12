using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.GoodsIssues;

/// <summary>
/// Phiếu xuất. Post mới trừ StockBalance và ghi InventoryTxn loại Out.
/// Hủy phiếu đã post thì hoàn tồn. Workflow post/hủy có thể tách MediatR sau.
/// </summary>
public class GoodsIssueService
    : Base.GenericCrudService<GoodsIssue, GoodsIssueDto, CreateGoodsIssueDto, UpdateGoodsIssueDto>,
      IGoodsIssueService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GoodsIssueService> _logger;

    public GoodsIssueService(
        IUnitOfWork unitOfWork,
        ILogger<GoodsIssueService> logger,
        IValidator<CreateGoodsIssueDto> createValidator,
        IValidator<UpdateGoodsIssueDto> updateValidator)
        : base(unitOfWork.GoodsIssues, unitOfWork, logger, createValidator, updateValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<GoodsIssueDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = _unitOfWork.GoodsIssues.Query();
            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Code.ToLower(), term) ||
                    EF.Functions.Like(x.SalesOrder.Number.ToLower(), term) ||
                    EF.Functions.Like(x.Warehouse.Name.ToLower(), term));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToGoodsIssueDto().PaginatedListAsync(search.PageNumber, search.PageSize, ct);
            return Result<PaginatedList<GoodsIssueDto>>.Success(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching goods issues");
            return Result<PaginatedList<GoodsIssueDto>>.CriticalError("Internal server error occurred while searching goods issues.");
        }
    }

    public async Task<Result> PostAsync(Guid id, CancellationToken ct = default)
    {
        var issue = await _unitOfWork.GoodsIssues.Query()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (issue is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(GoodsIssue), id));
        if (issue.Status != GoodsIssueStatus.Draft)
            return Result.Error("Chỉ ghi sổ phiếu đang nháp.");
        if (issue.Lines.Count == 0)
            return Result.Error("Phiếu xuất chưa có dòng.");

        var order = await _unitOfWork.SalesOrders.GetByIdAsync(issue.SalesOrderId, ct);
        if (order is null || order.Status != SalesOrderStatus.Confirmed)
            return Result.Error("Đơn bán phải đã xác nhận.");

        var lineIds = issue.Lines.Select(x => x.SalesOrderLineId).ToList();
        var orderLines = await _unitOfWork.SalesOrderLines.Query()
            .Where(x => lineIds.Contains(x.Id) && x.SalesOrderId == issue.SalesOrderId)
            .ToListAsync(ct);

        var issued = await _unitOfWork.GoodsIssueLines.Query()
            .Where(x => lineIds.Contains(x.SalesOrderLineId) && x.GoodsIssue.Status == GoodsIssueStatus.Posted)
            .GroupBy(x => x.SalesOrderLineId)
            .Select(g => new { LineId = g.Key, Qty = g.Sum(x => x.Quantity) })
            .ToListAsync(ct);

        foreach (var line in issue.Lines)
        {
            var orderLine = orderLines.FirstOrDefault(x => x.Id == line.SalesOrderLineId);
            if (orderLine is null)
                return Result.Error("Dòng phiếu không thuộc đơn bán.");
            if (orderLine.ProductId != line.ProductId)
                return Result.Error("Sản phẩm dòng phiếu không khớp dòng đơn.");

            var already = issued.FirstOrDefault(x => x.LineId == line.SalesOrderLineId)?.Qty ?? 0;
            if (already + line.Quantity > orderLine.Qty)
                return Result.Error("Số xuất vượt số còn lại của dòng đơn.");
        }

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            foreach (var line in issue.Lines)
            {
                var balance = await _unitOfWork.StockBalances.Query()
                    .FirstOrDefaultAsync(x => x.ProductId == line.ProductId && x.WarehouseId == issue.WarehouseId, ct);
                if (balance is null || balance.Qty < line.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync(ct);
                    return Result.Error("Không đủ tồn tại kho xuất.");
                }

                balance.Qty -= line.Quantity;
                await _unitOfWork.InventoryTxns.AddAsync(new InventoryTxn
                {
                    ProductId = line.ProductId,
                    WarehouseId = issue.WarehouseId,
                    Type = InventoryTxnType.Out,
                    QtyChange = -line.Quantity,
                    Reference = issue.Code,
                    Note = "Xuất kho bán hàng"
                }, ct);
            }

            issue.Status = GoodsIssueStatus.Posted;
            issue.PostedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            _logger.LogError(ex, "Error posting goods issue {Id}", id);
            return Result.Error("Không ghi sổ được phiếu xuất.");
        }
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var issue = await _unitOfWork.GoodsIssues.Query()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (issue is null)
            return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(GoodsIssue), id));
        if (issue.Status == GoodsIssueStatus.Cancelled)
            return Result.Error("Phiếu đã hủy.");

        if (issue.Status == GoodsIssueStatus.Draft)
        {
            issue.Status = GoodsIssueStatus.Cancelled;
            issue.CancelledAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            foreach (var line in issue.Lines)
            {
                var balance = await _unitOfWork.StockBalances.Query()
                    .FirstOrDefaultAsync(x => x.ProductId == line.ProductId && x.WarehouseId == issue.WarehouseId, ct);
                if (balance is null)
                {
                    await _unitOfWork.StockBalances.AddAsync(new StockBalance
                    {
                        ProductId = line.ProductId,
                        WarehouseId = issue.WarehouseId,
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
                    WarehouseId = issue.WarehouseId,
                    Type = InventoryTxnType.Out,
                    QtyChange = line.Quantity,
                    Reference = issue.Code,
                    Note = "Hủy phiếu xuất"
                }, ct);
            }

            issue.Status = GoodsIssueStatus.Cancelled;
            issue.CancelledAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            _logger.LogError(ex, "Error cancelling goods issue {Id}", id);
            return Result.Error("Không hủy được phiếu xuất.");
        }
    }

    protected override Expression<Func<GoodsIssue, GoodsIssueDto>> ToDto() => Extensions.ExpressionToDto();

    protected override GoodsIssue CreateEntity(CreateGoodsIssueDto dto)
    {
        var issue = new GoodsIssue
        {
            Code = dto.Code.Trim(),
            SalesOrderId = dto.SalesOrderId,
            WarehouseId = dto.WarehouseId,
            IssueDate = dto.IssueDate,
            Notes = dto.Notes,
            Status = GoodsIssueStatus.Draft
        };

        foreach (var line in dto.Lines)
        {
            var orderLine = _unitOfWork.SalesOrderLines.Query().First(x => x.Id == line.SalesOrderLineId);
            issue.Lines.Add(new GoodsIssueLine
            {
                SalesOrderLineId = line.SalesOrderLineId,
                ProductId = orderLine.ProductId,
                Quantity = line.Quantity,
                Note = line.Note
            });
        }

        return issue;
    }

    protected override Expression<Func<GoodsIssue, UpdateGoodsIssueDto>> ToUpdateDto() => x => new UpdateGoodsIssueDto
    {
        Id = x.Id,
        Code = x.Code,
        WarehouseId = x.WarehouseId,
        IssueDate = x.IssueDate,
        Notes = x.Notes
    };

    protected override void UpdateEntity(GoodsIssue entity, UpdateGoodsIssueDto dto)
    {
        if (entity.Status != GoodsIssueStatus.Draft)
            throw new InvalidOperationException("Chỉ sửa phiếu đang nháp.");

        if (dto.Code.HasValueAndIsDifferentFrom(entity.Code))
            entity.Code = dto.Code!.Trim();
        if (dto.WarehouseId.HasValueAndIsDifferentFrom(entity.WarehouseId))
            entity.WarehouseId = dto.WarehouseId!.Value;
        if (dto.IssueDate.HasValueAndIsDifferentFrom(entity.IssueDate))
            entity.IssueDate = dto.IssueDate!.Value;
        if (dto.Notes.IsDifferentFrom(entity.Notes))
            entity.Notes = dto.Notes;
    }
}
