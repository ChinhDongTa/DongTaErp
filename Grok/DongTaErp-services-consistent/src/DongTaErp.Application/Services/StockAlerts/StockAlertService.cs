using System.Text;
using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.StockAlerts;

public class StockAlertService(IUnitOfWork unitOfWork, IAppEmailSender email, ILogger<StockAlertService> logger)
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

        var recipients = await GetRecipientsAsync(ct);
        if (recipients.Count == 0)
            return Result<int>.Error("Chưa có nhân viên có email được phân quyền xử lý hoặc ban giám đốc.");

        await email.SendAsync(recipients.Select(x => x.Email), $"Cảnh báo tồn kho: {alerts.Count} sản phẩm cần xử lý", BuildBody(alerts, recipients), ct);
        logger.LogInformation("Sent stock alert to {Count} employees", recipients.Count);
        return Result<int>.Success(alerts.Count);
    }

    private async Task<List<Recipient>> GetRecipientsAsync(CancellationToken ct)
    {
        var dutyPositionIds = await unitOfWork.PositionDuties.Query()
            .Where(x => x.Topic == WorkTopic.StockAlert)
            .Select(x => x.JobPositionId)
            .ToListAsync(ct);
        var assignedIds = await unitOfWork.WorkAssignments.Query()
            .Where(x => x.Topic == WorkTopic.StockAlert)
            .Select(x => x.EmployeeId)
            .ToListAsync(ct);

        return await unitOfWork.Employees.Query()
            .Where(x => x.Email != null && x.Email != "" && x.Status != EmployeeStatus.Resigned)
            .Where(x => x.Position.IsDirector || dutyPositionIds.Contains(x.PositionId) || assignedIds.Contains(x.Id))
            .Select(x => new Recipient(x.Email!, x.FullName, x.Position.IsDirector ? "Ban giám đốc" : "Xử lý"))
            .ToListAsync(ct);
    }

    private static string BuildBody(List<StockAlertDto> alerts, List<Recipient> recipients)
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
        sb.AppendLine("Người nhận:");
        foreach (var recipient in recipients.DistinctBy(x => x.Email))
            sb.AppendLine($"- {recipient.Name} ({recipient.Role})");
        return sb.ToString();
    }

    private sealed record Recipient(string Email, string Name, string Role);
}