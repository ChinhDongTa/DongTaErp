namespace DongTaErp.Application.Services.StockAlerts;

public interface IStockAlertService
{
    Task<Result<List<StockAlertDto>>> GetAlertsAsync(CancellationToken ct = default);
    Task<Result<int>> NotifyPurchasingAsync(CancellationToken ct = default);
}
