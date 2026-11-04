using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.WorkAssignments;

public class WorkAssignmentService(IUnitOfWork unitOfWork) : IWorkAssignmentService
{
    public async Task<Result<WorkSetupDto>> GetStockSetupAsync(CancellationToken ct = default)
    {
        var dto = new WorkSetupDto
        {
            Positions = await unitOfWork.JobPositions.Query().Where(x => x.IsActive).OrderBy(x => x.Name)
                .Select(x => new WorkRecipientDto { Id = x.Id, Name = x.Name }).ToListAsync(ct),
            Employees = await unitOfWork.Employees.Query().Where(x => x.Status != EmployeeStatus.Resigned).OrderBy(x => x.FullName)
                .Select(x => new WorkRecipientDto { Id = x.Id, Name = x.Code + " - " + x.FullName, Email = x.Email }).ToListAsync(ct),
            DutyPositions = await unitOfWork.PositionDuties.Query().Where(x => x.Topic == WorkTopic.StockAlert)
                .Select(x => new WorkRecipientDto { Id = x.Id, Name = x.JobPosition.Name }).ToListAsync(ct),
            AssignedEmployees = await unitOfWork.WorkAssignments.Query().Where(x => x.Topic == WorkTopic.StockAlert)
                .Select(x => new WorkRecipientDto { Id = x.Id, Name = x.Employee.FullName, Email = x.Employee.Email }).ToListAsync(ct)
        };
        return Result<WorkSetupDto>.Success(dto);
    }

    public async Task<Result> AddPositionDutyAsync(Guid jobPositionId, CancellationToken ct = default)
    {
        if (await unitOfWork.PositionDuties.Query().AnyAsync(x => x.JobPositionId == jobPositionId && x.Topic == WorkTopic.StockAlert, ct))
            return Result.Success(ResultStatus.NoContent);
        await unitOfWork.PositionDuties.AddAsync(new PositionDuty { JobPositionId = jobPositionId, Topic = WorkTopic.StockAlert }, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> RemovePositionDutyAsync(Guid id, CancellationToken ct = default)
    {
        var duty = await unitOfWork.PositionDuties.GetByIdAsync(id, ct);
        if (duty is null)
            return Result.NotFound("Không tìm thấy phân quyền.");
        unitOfWork.PositionDuties.Delete(duty);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> AddEmployeeAssignmentAsync(Guid employeeId, CancellationToken ct = default)
    {
        if (await unitOfWork.WorkAssignments.Query().AnyAsync(x => x.EmployeeId == employeeId && x.Topic == WorkTopic.StockAlert, ct))
            return Result.Success(ResultStatus.NoContent);
        await unitOfWork.WorkAssignments.AddAsync(new WorkAssignment { EmployeeId = employeeId, Topic = WorkTopic.StockAlert }, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }

    public async Task<Result> RemoveEmployeeAssignmentAsync(Guid id, CancellationToken ct = default)
    {
        var item = await unitOfWork.WorkAssignments.GetByIdAsync(id, ct);
        if (item is null)
            return Result.NotFound("Không tìm thấy phân công.");
        unitOfWork.WorkAssignments.Delete(item);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ResultStatus.NoContent);
    }
}