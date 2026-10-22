using DongTaErp.Application.Common.DTOs;
using DongTaErp.Application.Common.Interfaces;
using DongTaErp.Application.Common.Models;
using DongTaErp.Application.Extensions;
using DongTaErp.Domain.Entities;
using DongTaErp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
}
