using DongTaErp.Application.Common.DTOs;
using DongTaErp.Application.Common.Models;

namespace DongTaErp.Application.Services.Dashboard;

public interface IDashboardService
{
    Task<Result<DashboardDto>> GetAsync(CancellationToken ct = default);
    Task<Result<List<BuySuggestionDto>>> GetBuySuggestionsAsync(CancellationToken ct = default);
}
