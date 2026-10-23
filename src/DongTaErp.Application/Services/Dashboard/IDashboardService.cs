namespace DongTaErp.Application.Services.Dashboard;

public interface IDashboardService
{
    Task<Result<DashboardDto>> GetAsync(CancellationToken ct = default);
    Task<Result<TransferReviewDto?>> GetNextTransferReviewAsync(CancellationToken ct = default);
    Task<Result<List<BuySuggestionDto>>> GetBuySuggestionsAsync(CancellationToken ct = default);
}
