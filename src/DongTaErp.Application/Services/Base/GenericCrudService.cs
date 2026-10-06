namespace DongTaErp.Application.Services.Base;

using System.Linq.Expressions;

public abstract class GenericCrudService<TEntity, TDto, TCreateDto, TUpdateDto> : ICrudService<TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseAuditableEntity
    where TDto : BaseDto
{
    protected readonly IRepository<TEntity> Repository;
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly ILogger Logger;

    protected GenericCrudService(IRepository<TEntity> repository, IUnitOfWork unitOfWork, ILogger logger)
    {
        Repository = repository;
        UnitOfWork = unitOfWork;
        Logger = logger;
    }

    public virtual async Task<Result<TDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var dto = await Repository.Query()
                .Where(x => x.Id == id)
                .Select(ToDto())
                .FirstOrDefaultAsync(ct);

            return dto == null ? Result<TDto?>.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id)) : Result<TDto?>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting {Entity}", typeof(TEntity).Name);

            return Result<TDto?>.CriticalError($"Error getting {typeof(TEntity).Name}");
        }
    }

    public virtual async Task<Result<PaginatedList<TDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        try
        {
            var result = await Repository.Query()
                .OrderByDescending(x => x.Created)
                .Select(ToDto())
                .PaginatedListAsync(pageNumber, pageSize, ct);

            return Result<PaginatedList<TDto>>
                .Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting {Entity}", typeof(TEntity).Name);

            return Result<PaginatedList<TDto>>.CriticalError("System error");
        }
    }

    public virtual async Task<Result<string>> CreateAsync(TCreateDto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = CreateEntity(dto);

            await Repository.AddAsync(entity, ct);

            await UnitOfWork.SaveChangesAsync(ct);

            return Result<string>.Created(entity.Id.ToString());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating {Entity}", typeof(TEntity).Name);

            return Result<string>.CriticalError("System error");
        }
    }

    public virtual async Task<Result> UpdateAsync(Guid id, TUpdateDto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = await Repository.GetByIdAsync(id, ct);

            if (entity is null)
            {
                return Result.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id));
            }

            UpdateEntity(entity, dto);

            await UnitOfWork.SaveChangesAsync(ct);

            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating {Entity}", typeof(TEntity).Name);

            return Result.Error("System error");
        }
    }

    public virtual async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken ct = default)
    {
        try
        {
            var entity = await Repository.GetByIdAsync(id, ct);

            if (entity == null)
            {
                return Result.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id));
            }

            Repository.Delete(entity);

            await UnitOfWork.SaveChangesAsync(ct);

            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting {Entity}", typeof(TEntity).Name);

            return Result.Error("System error");
        }
    }

    protected abstract Expression<Func<TEntity, TDto>> ToDto();

    protected abstract TEntity CreateEntity(TCreateDto dto);
    protected abstract Expression<Func<TEntity, TUpdateDto>> ToUpdateDto();
    protected abstract void UpdateEntity(TEntity entity, TUpdateDto dto);

    public virtual async Task<Result<TUpdateDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var dto = await Repository.Query()
                .Where(x => x.Id == id)
                .Select(ToUpdateDto())
                .FirstOrDefaultAsync(ct);

            return dto == null
                ? Result<TUpdateDto?>.NotFound(ErrorHelpers.NotFoundWithId(typeof(TEntity).Name, id))
                : Result<TUpdateDto?>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting update dto");

            return Result<TUpdateDto?>.CriticalError("System error");
        }
    }
}