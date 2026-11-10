namespace DongTaErp.Application.Services.StockTransfers;

public interface IStockTransferService
{
    Task<Result<List<StockTransferOptionDto>>> GetProductsAsync(CancellationToken ct = default);
    Task<Result<List<StockTransferOptionDto>>> GetWarehousesAsync(CancellationToken ct = default);
    Task<Result> TransferAsync(CreateStockTransferDto dto, CancellationToken ct = default);
    Task<Result<List<StockTransferResultDto>>> CreateMinLevelTransfersAsync(CancellationToken ct = default);
}
