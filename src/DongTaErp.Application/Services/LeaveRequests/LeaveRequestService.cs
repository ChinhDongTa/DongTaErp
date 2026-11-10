using DongTaErp.Domain.Enums;

namespace DongTaErp.Application.Services.LeaveRequests;

/// <summary>
/// CRUD + search + duyệt/từ chối. Workflow nhiều cấp (nhân viên → quản lý → HR)
/// nên tách command MediatR/Wolverine khi cần audit trail và thông báo.
/// </summary>
public class LeaveRequestService(
    IUnitOfWork unitOfWork,
    ILogger<LeaveRequestService> logger,
    IValidator<CreateLeaveRequestDto> createValidator,
    IValidator<UpdateLeaveRequestDto> updateValidator) : ILeaveRequestService
{
    public Task<Result<LeaveRequestDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetByIdAsync(unitOfWork.LeaveRequests, ToDto(), id, logger, ct);

    public Task<Result<PaginatedList<LeaveRequestDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => CrudOperations.GetAllAsync(unitOfWork.LeaveRequests, ToDto(), pageNumber, pageSize, logger, ct);

    public Task<Result<string>> CreateAsync(CreateLeaveRequestDto dto, CancellationToken ct = default)
        => CrudOperations.CreateAsync(unitOfWork.LeaveRequests, unitOfWork, createValidator, CreateEntity, dto, logger, ct);

    public Task<Result> UpdateAsync(Guid id, UpdateLeaveRequestDto dto, CancellationToken ct = default)
        => CrudOperations.UpdateAsync(unitOfWork.LeaveRequests, unitOfWork, updateValidator, UpdateEntity, id, dto, logger, ct);

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.DeleteAsync(unitOfWork.LeaveRequests, unitOfWork, id, logger, ct);

    public Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.SoftDeleteAsync(unitOfWork.LeaveRequests, unitOfWork, id, logger, ct);

    public Task<Result<UpdateLeaveRequestDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
        => CrudOperations.GetUpdateByIdAsync(unitOfWork.LeaveRequests, ToUpdateDto(), id, logger, ct);


    public async Task<Result<PaginatedList<LeaveRequestDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.LeaveRequests.Query();

            if (!string.IsNullOrWhiteSpace(search.SearchTerm))
            {
                var term = $"%{search.SearchTerm.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Employee.FullName.ToLower(), term) ||
                    EF.Functions.Like(x.Reason.ToLower(), term) ||
                    (x.ReviewNote != null && EF.Functions.Like(x.ReviewNote.ToLower(), term)));
            }

            query = string.IsNullOrWhiteSpace(search.SortBy)
                ? query.ApplySorting()
                : query.ApplySorting(search.SortBy, search.IsDescending);

            var page = await query.ToLeaveRequestDto()
                .PaginatedListAsync(search.PageNumber, search.PageSize, ct);

            return Result<PaginatedList<LeaveRequestDto>>.Success(page);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching leave requests");
            return Result<PaginatedList<LeaveRequestDto>>.CriticalError("Internal server error occurred while searching leave requests.");
        }
    }

    public Task<Result> ApproveAsync(Guid id, string? reviewNote, CancellationToken ct = default) =>
        ReviewAsync(id, LeaveStatus.Approved, reviewNote, ct);

    public Task<Result> RejectAsync(Guid id, string? reviewNote, CancellationToken ct = default) =>
        ReviewAsync(id, LeaveStatus.Rejected, reviewNote, ct);

    private async Task<Result> ReviewAsync(Guid id, LeaveStatus status, string? reviewNote, CancellationToken ct)
    {
        try
        {
            var entity = await unitOfWork.LeaveRequests.GetByIdAsync(id, ct);
            if (entity is null)
                return Result.NotFound(ErrorHelpers.NotFoundWithId(nameof(LeaveRequest), id));

            if (entity.Status != LeaveStatus.Pending)
                return Result.Error("Chỉ duyệt đơn đang chờ.");

            entity.Status = status;
            entity.ReviewNote = reviewNote;
            entity.ReviewedAt = DateTime.UtcNow;
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reviewing leave request {Id}", id);
            return Result.Error("System error");
        }
    }

    private static Expression<Func<LeaveRequest, LeaveRequestDto>> ToDto() => Extensions.ExpressionToDto();

    private static LeaveRequest CreateEntity(CreateLeaveRequestDto dto) => new()
    {
        EmployeeId = dto.EmployeeId,
        Type = dto.Type,
        FromDate = dto.FromDate,
        ToDate = dto.ToDate,
        Days = dto.Days,
        Reason = dto.Reason.Trim(),
        Status = dto.Status,
        ReviewNote = dto.ReviewNote,
        ReviewedAt = dto.ReviewedAt
    };

    private static Expression<Func<LeaveRequest, UpdateLeaveRequestDto>> ToUpdateDto() => x => new UpdateLeaveRequestDto
    {
        Id = x.Id,
        EmployeeId = x.EmployeeId,
        Type = x.Type,
        FromDate = x.FromDate,
        ToDate = x.ToDate,
        Days = x.Days,
        Reason = x.Reason,
        Status = x.Status,
        ReviewNote = x.ReviewNote,
        ReviewedAt = x.ReviewedAt
    };

    private static void UpdateEntity(LeaveRequest entity, UpdateLeaveRequestDto dto)
    {
        if (dto.EmployeeId.HasValueAndIsDifferentFrom(entity.EmployeeId))
            entity.EmployeeId = dto.EmployeeId!.Value;

        if (dto.Type.HasValueAndIsDifferentFrom(entity.Type))
            entity.Type = dto.Type!.Value;

        if (dto.FromDate.HasValueAndIsDifferentFrom(entity.FromDate))
            entity.FromDate = dto.FromDate!.Value;

        if (dto.ToDate.HasValueAndIsDifferentFrom(entity.ToDate))
            entity.ToDate = dto.ToDate!.Value;

        if (dto.Days.HasValueAndIsDifferentFrom(entity.Days))
            entity.Days = dto.Days!.Value;

        if (dto.Reason.HasValueAndIsDifferentFrom(entity.Reason))
            entity.Reason = dto.Reason!.Trim();

        if (dto.Status.HasValueAndIsDifferentFrom(entity.Status))
            entity.Status = dto.Status!.Value;

        if (dto.ReviewNote.IsDifferentFrom(entity.ReviewNote))
            entity.ReviewNote = dto.ReviewNote;

        if (dto.ReviewedAt.IsDifferentFrom(entity.ReviewedAt))
            entity.ReviewedAt = dto.ReviewedAt;
    }
}
