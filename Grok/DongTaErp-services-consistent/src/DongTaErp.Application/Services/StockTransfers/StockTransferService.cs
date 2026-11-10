using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.StockTransfers;

public class StockTransferService(IUnitOfWork unitOfWork, ILogger<StockTransferService> logger) : IStockTransferService
{
    public async Task<Result<List<StockTransferOptionDto>>> GetProductsAsync(CancellationToken ct = default)
    {
        var items = await unitOfWork.Products.Query()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Sku)
            .Select(x => new StockTransferOptionDto { Id = x.Id, Label = x.Sku + " - " + x.Name })
            .ToListAsync(ct);
        return Result<List<StockTransferOptionDto>>.Success(items);
    }

    public async Task<Result<List<StockTransferOptionDto>>> GetWarehousesAsync(CancellationToken ct = default)
    {
        var items = await unitOfWork.Warehouses.Query()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new StockTransferOptionDto { Id = x.Id, Label = x.Code + " - " + x.Name })
            .ToListAsync(ct);
        return Result<List<StockTransferOptionDto>>.Success(items);
    }

    public async Task<Result> TransferAsync(CreateStockTransferDto dto, CancellationToken ct = default)
    {
        if (dto.ProductId == Guid.Empty || dto.FromWarehouseId == Guid.Empty || dto.ToWarehouseId == Guid.Empty)
            return Result.Error("Chọn đủ sản phẩm và hai kho.");
        if (dto.FromWarehouseId == dto.ToWarehouseId)
            return Result.Error("Kho nguồn và kho đích phải khác nhau.");
        if (dto.Qty <= 0)
            return Result.Error("Số lượng phải lớn hơn 0.");

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var source = await unitOfWork.StockBalances.Query()
                .FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && x.WarehouseId == dto.FromWarehouseId, ct);
            if (source is null || source.Qty < dto.Qty)
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                return Result.Error("Kho nguồn không đủ tồn.");
            }

            source.Qty -= dto.Qty;
            var target = await unitOfWork.StockBalances.Query()
                .FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && x.WarehouseId == dto.ToWarehouseId, ct);
            if (target is null)
            {
                target = new StockBalance
                {
                    ProductId = dto.ProductId,
                    WarehouseId = dto.ToWarehouseId,
                    Qty = dto.Qty,
                    MinQty = source.MinQty
                };
                await unitOfWork.StockBalances.AddAsync(target, ct);
            }
            else
            {
                target.Qty += dto.Qty;
            }

            var code = "CK".GetRandomCode();
            await unitOfWork.InventoryTxns.AddAsync(new InventoryTxn
            {
                ProductId = dto.ProductId,
                WarehouseId = dto.FromWarehouseId,
                ContraWarehouseId = dto.ToWarehouseId,
                Type = InventoryTxnType.Transfer,
                QtyChange = -dto.Qty,
                Reference = code,
                Note = dto.Notes
            }, ct);
            await unitOfWork.InventoryTxns.AddAsync(new InventoryTxn
            {
                ProductId = dto.ProductId,
                WarehouseId = dto.ToWarehouseId,
                ContraWarehouseId = dto.FromWarehouseId,
                Type = InventoryTxnType.Transfer,
                QtyChange = dto.Qty,
                Reference = code,
                Note = dto.Notes
            }, ct);

            await unitOfWork.SaveChangesAsync(ct);
            await unitOfWork.CommitTransactionAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            logger.LogError(ex, "Error transferring stock");
            return Result.Error("Không điều chuyển được.");
        }
    }

    public async Task<Result<List<StockTransferResultDto>>> CreateMinLevelTransfersAsync(CancellationToken ct = default)
    {
        var balances = await unitOfWork.StockBalances.Query()
            .Where(x => x.MinQty > 0)
            .Select(x => new TransferRow(x.ProductId, x.Product.Name, x.WarehouseId, x.Warehouse.Name, x.Qty, x.MinQty))
            .ToListAsync(ct);

        var plans = new List<(TransferRow Source, TransferRow Target, decimal Qty)>();
        foreach (var rows in balances.GroupBy(x => x.ProductId).Select(g => g.ToList()))
        {
            var surplus = rows.Select(x => new Surplus(x, x.Qty - x.MinQty * 2)).Where(x => x.Extra > 0).OrderByDescending(x => x.Extra).ToList();
            foreach (var target in rows.Where(x => x.Qty < x.MinQty).OrderBy(x => x.Qty))
            {
                var need = target.MinQty - target.Qty;
                while (need > 0)
                {
                    var source = surplus.FirstOrDefault(x => x.Row.WarehouseId != target.WarehouseId && x.Extra > 0);
                    if (source is null)
                        break;
                    var move = Math.Min(need, source.Extra);
                    plans.Add((source.Row, target, move));
                    source.Extra -= move;
                    need -= move;
                }
            }
        }

        if (plans.Count == 0)
            return Result<List<StockTransferResultDto>>.Error("Không có kho dư trên mức an toàn để điều chuyển.");

        await unitOfWork.BeginTransactionAsync(ct);
        var created = new List<StockTransferResultDto>();
        try
        {
            foreach (var plan in plans)
            {
                var source = await unitOfWork.StockBalances.Query()
                    .FirstAsync(x => x.ProductId == plan.Source.ProductId && x.WarehouseId == plan.Source.WarehouseId, ct);
                if (source.Qty < plan.Qty)
                {
                    await unitOfWork.RollbackTransactionAsync(ct);
                    return Result<List<StockTransferResultDto>>.Error($"Kho {plan.Source.Warehouse} không đủ tồn để chuyển {plan.Source.Product}.");
                }

                source.Qty -= plan.Qty;
                var target = await unitOfWork.StockBalances.Query()
                    .FirstOrDefaultAsync(x => x.ProductId == plan.Target.ProductId && x.WarehouseId == plan.Target.WarehouseId, ct);
                if (target is null)
                {
                    target = new StockBalance { ProductId = plan.Target.ProductId, WarehouseId = plan.Target.WarehouseId, Qty = plan.Qty, MinQty = plan.Target.MinQty };
                    await unitOfWork.StockBalances.AddAsync(target, ct);
                }
                else
                {
                    target.Qty += plan.Qty;
                }

                var code = "CK".GetRandomCode();
                await unitOfWork.InventoryTxns.AddAsync(new InventoryTxn
                {
                    ProductId = plan.Source.ProductId,
                    WarehouseId = plan.Source.WarehouseId,
                    ContraWarehouseId = plan.Target.WarehouseId,
                    Type = InventoryTxnType.Transfer,
                    QtyChange = -plan.Qty,
                    Reference = code,
                    Note = "Tự lập để đạt tồn tối thiểu"
                }, ct);
                await unitOfWork.InventoryTxns.AddAsync(new InventoryTxn
                {
                    ProductId = plan.Target.ProductId,
                    WarehouseId = plan.Target.WarehouseId,
                    ContraWarehouseId = plan.Source.WarehouseId,
                    Type = InventoryTxnType.Transfer,
                    QtyChange = plan.Qty,
                    Reference = code,
                    Note = "Tự lập để đạt tồn tối thiểu"
                }, ct);
                created.Add(new StockTransferResultDto
                {
                    Code = code,
                    Product = plan.Source.Product,
                    FromWarehouse = plan.Source.Warehouse,
                    ToWarehouse = plan.Target.Warehouse,
                    Qty = plan.Qty
                });
            }

            await unitOfWork.SaveChangesAsync(ct);
            await unitOfWork.CommitTransactionAsync(ct);
            return Result<List<StockTransferResultDto>>.Success(created);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            logger.LogError(ex, "Error creating min-level transfers");
            return Result<List<StockTransferResultDto>>.Error("Không lập được phiếu điều chuyển.");
        }
    }

    private sealed record TransferRow(Guid ProductId, string Product, Guid WarehouseId, string Warehouse, decimal Qty, decimal MinQty);
    private sealed class Surplus(TransferRow row, decimal extra) { public TransferRow Row { get; } = row; public decimal Extra { get; set; } = extra; }
}
