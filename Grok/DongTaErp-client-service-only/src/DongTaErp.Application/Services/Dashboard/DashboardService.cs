using DongTaErp.Application.Common.DTOs;
using DongTaErp.Application.Common.Interfaces;
using DongTaErp.Application.Common.Models;
using DongTaErp.Application.Extensions;
using DongTaErp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DongTaErp.Application.Services.Dashboard;

public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
{
    public async Task<Result<DashboardDto>> GetAsync(CancellationToken ct = default)
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var dueLimit = today.AddDays(7);
        var openInvoice = new[] { InvoiceStatus.Issued, InvoiceStatus.PartiallyPaid };
        var countedInvoice = new[] { InvoiceStatus.Issued, InvoiceStatus.PartiallyPaid, InvoiceStatus.Paid };
        var openBill = new[] { VendorBillStatus.Posted, VendorBillStatus.PartiallyPaid };

        var dto = new DashboardDto
        {
            SalesWeek = await unitOfWork.Invoices.Query().Where(x => countedInvoice.Contains(x.Status) && x.IssueDate >= weekStart && x.IssueDate <= today).SumAsync(x => (decimal?)x.Total, ct) ?? 0,
            SalesMonth = await unitOfWork.Invoices.Query().Where(x => countedInvoice.Contains(x.Status) && x.IssueDate >= monthStart && x.IssueDate <= today).SumAsync(x => (decimal?)x.Total, ct) ?? 0
        };

        var balances = await unitOfWork.StockBalances.Query().Where(x => x.MinQty > 0)
            .Select(x => new BalanceRow(x.ProductId, x.Product.Name, x.WarehouseId, x.Warehouse.Name, x.Qty, x.MinQty)).ToListAsync(ct);
        dto.HighStock = balances.Where(x => x.Qty >= x.MinQty * 5).OrderByDescending(x => x.Qty / x.MinQty)
            .Select(x => new DashboardStockDto { Warehouse = x.Warehouse, Product = x.Product, Qty = x.Qty, MinQty = x.MinQty, Ratio = x.Qty / x.MinQty }).ToList();
        dto.Replenish = BuildBalance(balances);

        var receivable = await unitOfWork.Invoices.Query()
            .Where(x => openInvoice.Contains(x.Status) && x.DueDate != null && x.DueDate <= dueLimit && x.Total > x.PaidAmount)
            .Select(x => new DashboardDueDto { Kind = "Phải thu", Number = x.Number, Partner = x.Partner.Name, DueDate = x.DueDate!.Value, Balance = x.Total - x.PaidAmount, State = x.DueDate < today ? "Quá hạn" : "Tới hạn" }).ToListAsync(ct);
        var payable = await unitOfWork.VendorBills.Query()
            .Where(x => openBill.Contains(x.Status) && x.DueDate != null && x.DueDate <= dueLimit && x.Total > x.PaidAmount)
            .Select(x => new DashboardDueDto { Kind = "Phải trả", Number = x.Number, Partner = x.Partner.Name, DueDate = x.DueDate!.Value, Balance = x.Total - x.PaidAmount, State = x.DueDate < today ? "Quá hạn" : "Tới hạn" }).ToListAsync(ct);
        dto.Dues = receivable.Concat(payable).OrderBy(x => x.DueDate).ToList();
        dto.ReceivableDue = receivable.Sum(x => x.Balance);
        dto.PayableDue = payable.Sum(x => x.Balance);

        dto.TopProducts = await unitOfWork.InvoiceLines.Query()
            .Where(x => countedInvoice.Contains(x.Invoice.Status) && x.Invoice.IssueDate >= monthStart && x.Invoice.IssueDate <= today)
            .GroupBy(x => new { x.Product.Sku, x.Product.Name })
            .Select(g => new DashboardTopProductDto { Sku = g.Key.Sku, Name = g.Key.Name, Qty = g.Sum(x => x.Qty), Amount = g.Sum(x => x.LineTotal) })
            .OrderByDescending(x => x.Qty).Take(5).ToListAsync(ct);
        return Result<DashboardDto>.Success(dto);
    }

    private static List<DashboardReplenishDto> BuildBalance(List<BalanceRow> all)
    {
        var result = new List<DashboardReplenishDto>();
        foreach (var rows in all.GroupBy(x => x.ProductId).Select(g => g.ToList()))
        {
            if (rows.All(x => x.Qty >= x.MinQty * 2))
                continue;
            var surplus = rows.Select(x => new Surplus(x, x.Qty - x.MinQty * 2)).Where(x => x.Extra > 0).OrderByDescending(x => x.Extra).ToList();
            foreach (var target in rows.Where(x => x.Qty < x.MinQty * 2).OrderBy(x => x.Qty / x.MinQty))
            {
                var need = target.MinQty * 2 - target.Qty;
                var source = surplus.FirstOrDefault(x => x.Row.WarehouseId != target.WarehouseId && x.Extra > 0);
                if (source is not null)
                {
                    var move = Math.Min(need, source.Extra);
                    var remain = need - move;
                    result.Add(new DashboardReplenishDto
                    {
                        Warehouse = target.Warehouse, Product = target.Product, Qty = target.Qty, Shortage = need,
                        Action = remain == 0 ? "Cân bằng" : target.Qty < target.MinQty ? "Cân bằng + mua" : "Cân bằng",
                        Detail = remain == 0 ? $"Chuyển {move.ToDisplay()} từ {source.Row.Warehouse}" : $"Chuyển {move.ToDisplay()} từ {source.Row.Warehouse}, mua thêm {remain.ToDisplay()}",
                        ProductId = target.ProductId, FromWarehouseId = source.Row.WarehouseId, ToWarehouseId = target.WarehouseId, TransferQty = move, BuyQty = remain
                    });
                    source.Extra -= move;
                    continue;
                }
                if (target.Qty < target.MinQty)
                    result.Add(new DashboardReplenishDto
                    {
                        Warehouse = target.Warehouse, Product = target.Product, Qty = target.Qty, Shortage = target.MinQty - target.Qty,
                        Action = "Mua mới", Detail = $"Không có kho dư. Mua {(target.MinQty - target.Qty).ToDisplay()}",
                        ProductId = target.ProductId, ToWarehouseId = target.WarehouseId, BuyQty = target.MinQty - target.Qty
                    });
            }
        }
        return result.OrderByDescending(x => x.Shortage).ToList();
    }

    private sealed record BalanceRow(Guid ProductId, string Product, Guid WarehouseId, string Warehouse, decimal Qty, decimal MinQty);
    private sealed class Surplus(BalanceRow row, decimal extra) { public BalanceRow Row { get; } = row; public decimal Extra { get; set; } = extra; }
}
