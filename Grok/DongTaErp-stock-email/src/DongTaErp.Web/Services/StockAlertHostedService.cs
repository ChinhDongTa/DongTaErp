using DongTaErp.Application.Common.Email;
using DongTaErp.Application.Services.StockAlerts;
using Microsoft.Extensions.Options;

public class StockAlertHostedService(IServiceScopeFactory scopeFactory, IOptions<EmailOptions> options, ILogger<StockAlertHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var hours = Math.Max(1, options.Value.CheckHours);
        using var timer = new PeriodicTimer(TimeSpan.FromHours(hours));
        await CheckAsync(stoppingToken);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            await CheckAsync(stoppingToken);
    }

    private async Task CheckAsync(CancellationToken ct)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<IStockAlertService>();
            var result = await service.NotifyPurchasingAsync(ct);
            if (!result.Succeeded)
                logger.LogWarning("Stock alert skipped: {Error}", result.Errors?.FirstOrDefault());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Stock alert check failed");
        }
    }
}
