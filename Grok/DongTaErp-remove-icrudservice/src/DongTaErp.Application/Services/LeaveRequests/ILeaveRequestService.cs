namespace DongTaErp.Application.Services.LeaveRequests;

public interface ILeaveRequestService
{
    Task<Result<LeaveRequestDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<UpdateLeaveRequestDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PaginatedList<LeaveRequestDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<Result<string>> CreateAsync(CreateLeaveRequestDto dto, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateLeaveRequestDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<LeaveRequestDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> ApproveAsync(Guid id, string? reviewNote, CancellationToken ct = default);
    Task<Result> RejectAsync(Guid id, string? reviewNote, CancellationToken ct = default);
}
