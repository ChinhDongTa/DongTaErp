namespace DongTaErp.Application.Services.Dashboard;

public interface IDashboardService
{
    Task<Result<DashboardDto>> GetAsync(CancellationToken ct = default);
}
