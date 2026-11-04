namespace DongTaErp.Application.Services.WorkAssignments;

public interface IWorkAssignmentService
{
    Task<Result<WorkSetupDto>> GetStockSetupAsync(CancellationToken ct = default);
    Task<Result> AddPositionDutyAsync(Guid jobPositionId, CancellationToken ct = default);
    Task<Result> RemovePositionDutyAsync(Guid id, CancellationToken ct = default);
    Task<Result> AddEmployeeAssignmentAsync(Guid employeeId, CancellationToken ct = default);
    Task<Result> RemoveEmployeeAssignmentAsync(Guid id, CancellationToken ct = default);
}
