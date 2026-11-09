namespace DongTaErp.Application.Services.Employees;

public class EmployeeService(IUnitOfWork unitOfWork, ILogger<EmployeeService> logger) : IEmployeeService
{
    public async Task<Result<EmployeeDto?>> GetEmployeeByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await unitOfWork.Employees.Query()
                                               .Where(e => e.Id == id)
                                               .ToEmployeeDto()
                                               .SingleOrNotFoundAsync("Employee", id, ct);
        return result;
    }

    public async Task<Result<PaginatedList<EmployeeDto>>> GetAllEmployeesAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        try
        {
            var paginated = await unitOfWork.Employees.Query()
                .ApplySorting() // Default sorting by Created descending
                .ToEmployeeDto()
                .PaginatedListAsync(pageNumber, pageSize, ct);

            return Result<PaginatedList<EmployeeDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching employees.");
            return Result<PaginatedList<EmployeeDto>>.CriticalError("Internal server error occurred while fetching employees.");
        }
    }

    public async Task<Result<PaginatedList<EmployeeDto>>> SearchAsync(string? searchTerm,
                                                                        int pageNumber = 1,
                                                                        int pageSize = 20,
                                                                        string? sortBy = null,
                                                                        bool isDescending = false,
                                                                        CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.Employees.Query();

            // Build search query
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = $"%{searchTerm.ToLower()}%";
                query = query.Where(e => EF.Functions.Like(e.Code.ToLower(), lowerSearchTerm) ||
                                          EF.Functions.Like(e.FullName.ToLower(), lowerSearchTerm) ||
                                          (e.Email != null && EF.Functions.Like(e.Email.ToLower(), lowerSearchTerm)) ||
                                          (e.Phone != null && EF.Functions.Like(e.Phone.ToLower(), lowerSearchTerm)) ||
                                          EF.Functions.Like(e.IdentityNumber.ToLower(), lowerSearchTerm));
            }

            // Apply sorting
            query = sortBy != null ? query.ApplySorting(sortBy, isDescending) : query.ApplySorting();

            // Execute query and return paginated result
            var paginatedList = await query.ToEmployeeDto().PaginatedListAsync(pageNumber, pageSize, ct);
            return Result<PaginatedList<EmployeeDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching employees.");
            return Result<PaginatedList<EmployeeDto>>.CriticalError("Internal server error occurred while searching employees.");
        }
    }

    public async Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeDto dto, CancellationToken ct = default)
    {
        try
        {
            var employee = new Employee
            {
                Code = dto.Code.Trim(),
                FullName = dto.FullName.Trim(),
                Email = dto.Email,
                Phone = dto.Phone,
                IdentityNumber = dto.IdentityNumber.Trim(),
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                HireDate = dto.HireDate,
                ResignDate = dto.ResignDate,
                Gender = dto.Gender,
                Status = dto.Status,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                UserId = dto.UserId
            };

            await unitOfWork.Employees.AddAsync(employee, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<EmployeeDto>.Created(MapToDto(employee));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating a new employee.");
            return Result<EmployeeDto>.CriticalError("An error occurred while creating the employee.");
        }
    }

    public async Task<Result<EmployeeDto>> UpdateAsync(Guid id, UpdateEmployeeDto dto, CancellationToken ct = default)
    {
        try
        {
            var employee = await unitOfWork.Employees.GetByIdAsync(id, ct);
            if (employee == null)
                return Result<EmployeeDto>.NotFound(ErrorHelpers.NotFoundWithId("Employee", id));

            // Only update fields if they have changed
            if (dto.Code.HasValueAndIsDifferentFrom(employee.Code))
                employee.Code = dto.Code!.Trim();

            if (dto.FullName.HasValueAndIsDifferentFrom(employee.FullName))
                employee.FullName = dto.FullName!.Trim();

            if (dto.Email.IsDifferentFrom(employee.Email))
                employee.Email = dto.Email;

            if (dto.Phone.IsDifferentFrom(employee.Phone))
                employee.Phone = dto.Phone;

            if (dto.IdentityNumber.HasValueAndIsDifferentFrom(employee.IdentityNumber))
                employee.IdentityNumber = dto.IdentityNumber!.Trim();

            if (dto.Address.IsDifferentFrom(employee.Address))
                employee.Address = dto.Address;

            if (dto.BirthDate.IsDifferentFrom(employee.BirthDate))
                employee.BirthDate = dto.BirthDate;

            if (dto.HireDate.HasValueAndIsDifferentFrom(employee.HireDate))
                employee.HireDate = dto.HireDate!.Value;

            if (dto.ResignDate.IsDifferentFrom(employee.ResignDate))
                employee.ResignDate = dto.ResignDate;

            if (dto.Gender.HasValueAndIsDifferentFrom(employee.Gender))
                employee.Gender = dto.Gender!.Value;

            if (dto.Status.HasValueAndIsDifferentFrom(employee.Status))
                employee.Status = dto.Status!.Value;

            if (dto.DepartmentId.HasValueAndIsDifferentFrom(employee.DepartmentId))
                employee.DepartmentId = dto.DepartmentId!.Value;

            if (dto.PositionId.HasValueAndIsDifferentFrom(employee.PositionId))
                employee.PositionId = dto.PositionId!.Value;

            if (dto.UserId.IsDifferentFrom(employee.UserId))
                employee.UserId = dto.UserId;

            await unitOfWork.SaveChangesAsync(ct);

            return Result<EmployeeDto>.Success(MapToDto(employee));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the employee.");
            return Result<EmployeeDto>.CriticalError("An error occurred while updating the employee.");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var employee = await unitOfWork.Employees.GetByIdAsync(id, ct);
            if (employee == null)
                return Result.NotFound(ErrorHelpers.NotFoundWithId("Employee", id));

            unitOfWork.Employees.Delete(employee);
            await unitOfWork.SaveChangesAsync(ct);

            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the employee.");
            return Result.Error("An error occurred while deleting the employee.");
        }
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            Code = employee.Code,
            FullName = employee.FullName,
            Email = employee.Email,
            Phone = employee.Phone,
            IdentityNumber = employee.IdentityNumber,
            Address = employee.Address,
            BirthDate = employee.BirthDate,
            HireDate = employee.HireDate,
            ResignDate = employee.ResignDate,
            Gender = employee.Gender.ToString(),
            Status = employee.Status.ToString(),
            DepartmentName = employee.Department.Name,
            PositionName = employee.Position.Name,
            UserName = employee.User != null ? employee.User.UserName : null,
            CreatedAt = employee.Created
        };
    }
}
