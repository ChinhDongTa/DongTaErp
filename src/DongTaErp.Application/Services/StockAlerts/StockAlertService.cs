using System.Text;
using DongTaErp.Application.Common.Email;
using Microsoft.Extensions.Options;

namespace DongTaErp.Application.Services.StockAlerts;

public class StockAlertService(IUnitOfWork unitOfWork, IAppEmailSender email, IOptions<EmailOptions> options, ILogger<StockAlertService> logger)
    : IStockAlertService
{
    public async Task<Result<List<StockAlertDto>>> GetAlertsAsync(CancellationToken ct = default)
    {
        var rows = await unitOfWork.StockBalances.Query()
            .Where(x => x.MinQty > 0 && x.Qty < x.MinQty * 2)
            .OrderBy(x => x.Qty)
            .Select(x => new StockAlertDto
            {
                ProductId = x.ProductId,
                Sku = x.Product.Sku,
                ProductName = x.Product.Name,
                Warehouse = x.Warehouse.Code + " - " + x.Warehouse.Name,
                Qty = x.Qty,
                MinQty = x.MinQty,
                SafeQty = x.MinQty * 2,
                Level = x.Qty <= 0 ? "Hết hàng" : x.Qty < x.MinQty ? "Dưới tối thiểu" : "Dưới mức an toàn"
            })
            .ToListAsync(ct);
        return Result<List<StockAlertDto>>.Success(rows);
    }

    public async Task<Result<int>> NotifyPurchasingAsync(CancellationToken ct = default)
    {
        var alerts = (await GetAlertsAsync(ct)).Value ?? [];
        if (alerts.Count == 0)
            return Result<int>.Success(0);

        var recipients = options.Value.PurchasingRecipients
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (recipients.Count == 0)
            return Result<int>.Error("Chưa cấu hình email bộ phận mua hàng.");

        var body = BuildBody(alerts);
        await email.SendAsync(recipients, $"Cảnh báo tồn kho: {alerts.Count} sản phẩm cần xử lý", body, ct);
        logger.LogInformation("Sent stock alert to {Count} purchasing recipients", recipients.Count);
        return Result<int>.Success(alerts.Count);
    }

    private static string BuildBody(List<StockAlertDto> alerts)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Các sản phẩm dưới mức an toàn (2 lần tồn tối thiểu):");
        sb.AppendLine();
        foreach (var group in alerts.GroupBy(x => x.Level))
        {
            sb.AppendLine(group.Key);
            foreach (var item in group)
                sb.AppendLine($"- {item.Sku} {item.ProductName} | {item.Warehouse} | tồn {item.Qty:0.##} / tối thiểu {item.MinQty:0.##} / an toàn {item.SafeQty:0.##}");
            sb.AppendLine();
        }
        sb.AppendLine("Vui lòng điều chuyển hoặc lập đơn mua.");
        return sb.ToString();
    }
}
