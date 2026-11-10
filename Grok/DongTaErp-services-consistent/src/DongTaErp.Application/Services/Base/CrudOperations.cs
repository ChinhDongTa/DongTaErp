using FluentValidation;
using FluentValidation.Results;

namespace DongTaErp.Application.Services.Base;

/// <summary>
/// Các thao tác CRUD dùng chung, theo cùng kiểu xử lý của EmployeeService.
/// Service cụ thể gọi helper này, không kế thừa GenericCrudService.
/// </summary>
public static class CrudOperations
{
    public static async Task<Result<TDto?>> GetByIdAsync<TEntity, TDto>(
        IRepository<TEntity> repository,
        Expression<Func<TEntity, TDto>> toDto,
        Guid id,
        ILogger logger,
        CancellationToken ct = default)
        where TEntity : BaseAuditableEntity
    {
        try
        {
            var dto = await repository.Query().Where(x => x.Id == id).Select(toDto).FirstOrDefaultAsync(ct);
            return dto == null
                ? Result<TDto?>.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id))
                : Result<TDto?>.Success(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting {Entity}", typeof(TEntity).Name);
            return Result<TDto?>.CriticalError($"Error getting {typeof(TEntity).Name}");
        }
    }

    public static async Task<Result<PaginatedList<TDto>>> GetAllAsync<TEntity, TDto>(
        IRepository<TEntity> repository,
        Expression<Func<TEntity, TDto>> toDto,
        int pageNumber,
        int pageSize,
        ILogger logger,
        CancellationToken ct = default)
        where TEntity : BaseAuditableEntity
        where TDto : class
    {
        try
        {
            var result = await repository.Query()
                .OrderByDescending(x => x.Created)
                .Select(toDto)
                .PaginatedListAsync(pageNumber, pageSize, ct);
            return Result<PaginatedList<TDto>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting {Entity}", typeof(TEntity).Name);
            return Result<PaginatedList<TDto>>.CriticalError("System error");
        }
    }

    public static async Task<Result<string>> CreateAsync<TEntity, TCreateDto>(
        IRepository<TEntity> repository,
        IUnitOfWork unitOfWork,
        IValidator<TCreateDto> validator,
        Func<TCreateDto, TEntity> createEntity,
        TCreateDto dto,
        ILogger logger,
        CancellationToken ct = default)
        where TEntity : BaseAuditableEntity
    {
        try
        {
            var errors = await ValidateAsync(validator, dto, ct);
            if (errors.Count > 0)
                return Result<string>.Error([.. errors]);

            var entity = createEntity(dto);
            await repository.AddAsync(entity, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result<string>.Created(entity.Id.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating {Entity}", typeof(TEntity).Name);
            return Result<string>.CriticalError("System error");
        }
    }

    public static async Task<Result> UpdateAsync<TEntity, TUpdateDto>(
        IRepository<TEntity> repository,
        IUnitOfWork unitOfWork,
        IValidator<TUpdateDto> validator,
        Action<TEntity, TUpdateDto> updateEntity,
        Guid id,
        TUpdateDto dto,
        ILogger logger,
        CancellationToken ct = default)
        where TEntity : BaseAuditableEntity
    {
        try
        {
            var errors = await ValidateAsync(validator, dto, ct);
            if (errors.Count > 0)
                return Result.Error([.. errors]);

            var entity = await repository.GetByIdAsync(id, ct);
            if (entity is null)
                return Result.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id));

            updateEntity(entity, dto);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating {Entity}", typeof(TEntity).Name);
            return Result.Error("System error");
        }
    }

    public static async Task<Result> DeleteAsync<TEntity>(
        IRepository<TEntity> repository,
        IUnitOfWork unitOfWork,
        Guid id,
        ILogger logger,
        CancellationToken ct = default)
        where TEntity : BaseAuditableEntity
    {
        try
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null)
                return Result.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id));

            repository.Delete(entity);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting {Entity}", typeof(TEntity).Name);
            return Result.Error("System error");
        }
    }

    public static async Task<Result> SoftDeleteAsync<TEntity>(
        IRepository<TEntity> repository,
        IUnitOfWork unitOfWork,
        Guid id,
        ILogger logger,
        CancellationToken ct = default)
        where TEntity : BaseAuditableEntity
    {
        try
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity == null)
                return Result.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id));

            repository.SoftDelete(entity);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error soft deleting {Entity}", typeof(TEntity).Name);
            return Result.Error("System error");
        }
    }

    public static async Task<Result<TUpdateDto?>> GetUpdateByIdAsync<TEntity, TUpdateDto>(
        IRepository<TEntity> repository,
        Expression<Func<TEntity, TUpdateDto>> toUpdateDto,
        Guid id,
        ILogger logger,
        CancellationToken ct = default)
        where TEntity : BaseAuditableEntity
    {
        try
        {
            var dto = await repository.Query().Where(x => x.Id == id).Select(toUpdateDto).FirstOrDefaultAsync(ct);
            return dto == null
                ? Result<TUpdateDto?>.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id))
                : Result<TUpdateDto?>.Success(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting update dto");
            return Result<TUpdateDto?>.CriticalError("System error");
        }
    }

    public static async Task<IReadOnlyList<string>> ValidateAsync<T>(IValidator<T> validator, T dto, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(dto, ct);
        return ToErrors(result);
    }

    public static IReadOnlyList<string> ToErrors(ValidationResult result) => result.IsValid
        ? []
        : [.. result.Errors.Select(e => string.IsNullOrWhiteSpace(e.PropertyName) ? e.ErrorMessage : $"{e.PropertyName}: {e.ErrorMessage}")];
}
