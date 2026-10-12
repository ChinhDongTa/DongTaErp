namespace DongTaErp.Application.Services.LeaveRequests;

public interface ILeaveRequestService : ICrudService<LeaveRequestDto, CreateLeaveRequestDto, UpdateLeaveRequestDto>
{
    Task<Result<PaginatedList<LeaveRequestDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);
    Task<Result> ApproveAsync(Guid id, string? reviewNote, CancellationToken ct = default);
    Task<Result> RejectAsync(Guid id, string? reviewNote, CancellationToken ct = default);
}
