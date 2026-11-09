// TEMPLATE: Copy this file to Services/[Entity]/[Entity]Service.cs and replace [Entity] with your entity name
// Also create Extensions.cs and update I[Entity]Service.cs interface accordingly

using Microsoft.Extensions.Logging;

namespace DongTaErp.Application.Services.[Entities];

/// <summary>
/// CRUD and search service for [EntityPlural] following the standardized Application layer pattern.
/// 
/// Pattern Notes:
/// - Primary constructor with IUnitOfWork and ILogger only (no separate validators)
/// - Projection via To[Entity]Dto() extension method
/// - Sorting via ApplySorting() extension method with reusable OrderBy<TKey>() helper
/// - Pagination via PaginatedListAsync() extension
/// - Patch-style updates: only fields with HasValueAndIsDifferentFrom / IsDifferentFrom are updated
/// - All public methods return Result<T> or Result
/// - Exception logging includes detailed context but error responses don't expose internals
/// </summary>
public class [Entity]Service(IUnitOfWork unitOfWork, ILogger<[Entity]Service> logger) : I[Entity]Service
{
    /// <summary>
    /// Gets a single [entity] by ID, projected to [Entity]Dto.
    /// </summary>
    /// <remarks>
    /// Uses SingleOrNotFoundAsync which ensures only one result or NotFound.
    /// Extension handles both the query projection and the result wrapping.
    /// </remarks>
    public async Task<Result<[Entity]Dto?>> Get[Entity]ByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await unitOfWork.[Entities].Query()
            .Where(e => e.Id == id)
            .To[Entity]Dto()
            .SingleOrNotFoundAsync("[Entity]", id, ct);
        return result;
    }

    /// <summary>
    /// Gets all [entities] with default sorting (by Created descending, newest first).
    /// </summary>
    /// <param name="pageNumber">1-based page number; default is 1</param>
    /// <param name="pageSize">Number of records per page; default is 20</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>
    /// Success with paginated list of [Entity]Dto if query succeeds.
    /// CriticalError if an unexpected exception occurs.
    /// </returns>
    public async Task<Result<PaginatedList<[Entity]Dto>>> GetAll[Entities]Async(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        try
        {
            var paginated = await unitOfWork.[Entities].Query()
                .ApplySorting() // Default sorting by Created descending
                .To[Entity]Dto()
                .PaginatedListAsync(pageNumber, pageSize, ct);

            return Result<PaginatedList<[Entity]Dto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching [entities].");
            return Result<PaginatedList<[Entity]Dto>>.CriticalError("Internal server error occurred while fetching [entities].");
        }
    }

    /// <summary>
    /// Searches [entities] by term and applies optional sorting.
    /// </summary>
    /// <param name="searchTerm">Searches in: [CUSTOMIZE: e.g., "Code, Name, Phone, Email"]. Null or empty skips search filtering.</param>
    /// <param name="pageNumber">1-based page number; default is 1</param>
    /// <param name="pageSize">Number of records per page; default is 20</param>
    /// <param name="sortBy">Supported values: [CUSTOMIZE: e.g., "code", "name", "phone", "email"]. Default: creation date descending.</param>
    /// <param name="isDescending">If true, sort descending; else ascending</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>
    /// Success with paginated search results if query succeeds.
    /// CriticalError if an unexpected exception occurs.
    /// </returns>
    public async Task<Result<PaginatedList<[Entity]Dto>>> Search[Entities]Async(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        try
        {
            var query = unitOfWork.[Entities].Query();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = $"%{searchTerm.ToLower()}%";
                // CUSTOMIZE: Add searchable fields below
                query = query.Where(e =>
                    EF.Functions.Like(e.[SearchField1].ToLower(), lowerSearchTerm) ||
                    EF.Functions.Like(e.[SearchField2].ToLower(), lowerSearchTerm) ||
                    (e.[NullableSearchField] != null && EF.Functions.Like(e.[NullableSearchField].ToLower(), lowerSearchTerm)));
            }

            // Apply sorting and pagination
            var paginatedList = await query
                .ApplySorting(sortBy, isDescending)
                .To[Entity]Dto()
                .PaginatedListAsync(pageNumber, pageSize, ct);

            return Result<PaginatedList<[Entity]Dto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while searching [entities].");
            return Result<PaginatedList<[Entity]Dto>>.CriticalError("Internal server error occurred while searching [entities].");
        }
    }

    /// <summary>
    /// Creates a new [entity].
    /// </summary>
    /// <param name="dto">Create [entity] DTO with required fields</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>
    /// Created with the created [Entity]Dto and 201 status if successful.
    /// CriticalError if an unexpected exception occurs.
    /// </returns>
    public async Task<Result<[Entity]Dto>> Create[Entity]Async(Create[Entity]Dto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = new [Entity]
            {
                // CUSTOMIZE: Map DTO fields to entity
                // Code = dto.Code,
                // Name = dto.Name,
            };

            await unitOfWork.[Entities].AddAsync(entity, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("[Entity] created with ID {Id}", entity.Id);
            return Result<[Entity]Dto>.Created(MapToDto(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating a new [entity].");
            return Result<[Entity]Dto>.CriticalError("An error occurred while creating the [entity].");
        }
    }

    /// <summary>
    /// Updates an existing [entity] with patch-style semantics.
    /// Only fields that are non-null and different from current values are updated.
    /// </summary>
    /// <param name="id">ID of [entity] to update</param>
    /// <param name="dto">Update [entity] DTO with optional fields (nullable types allow selective updates)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>
    /// Success with updated [Entity]Dto if update succeeds.
    /// NotFound if [entity] with given ID doesn't exist.
    /// CriticalError if an unexpected exception occurs.
    /// </returns>
    public async Task<Result<[Entity]Dto>> Update[Entity]Async(Guid id, Update[Entity]Dto dto, CancellationToken ct = default)
    {
        try
        {
            var entity = await unitOfWork.[Entities].GetByIdAsync(id, ct);
            if (entity == null)
                return Result<[Entity]Dto>.NotFound(ErrorHelpers.NotFoundWithId("[Entity]", id));

            // Patch-style update: only update if field changed
            // CUSTOMIZE: Add field updates below. Use HasValueAndIsDifferentFrom for required string fields,
            // or IsDifferentFrom for optional/value-type fields.
            // Example:
            // if (dto.Code.HasValueAndIsDifferentFrom(entity.Code))
            //     entity.Code = dto.Code!;
            // if (dto.IsActive.IsDifferentFrom(entity.IsActive))
            //     entity.IsActive = dto.IsActive;

            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("[Entity] updated with ID {Id}", id);
            return Result<[Entity]Dto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the [entity].");
            return Result<[Entity]Dto>.CriticalError("An error occurred while updating the [entity].");
        }
    }

    /// <summary>
    /// Deletes an [entity] by ID.
    /// </summary>
    /// <param name="id">ID of [entity] to delete</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>
    /// Success with NoContent status if deletion succeeds.
    /// NotFound if [entity] with given ID doesn't exist.
    /// Error if an unexpected exception occurs.
    /// </returns>
    public async Task<Result> Delete[Entity]Async(Guid id, CancellationToken ct = default)
    {
        try
        {
            var entity = await unitOfWork.[Entities].GetByIdAsync(id, ct);
            if (entity == null)
                return Result.NotFound(ErrorHelpers.NotFoundWithId("[Entity]", id));

            unitOfWork.[Entities].Delete(entity);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("[Entity] deleted with ID {Id}", id);
            return Result.Success(ResultStatus.NoContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the [entity].");
            return Result.Error("An error occurred while deleting the [entity].");
        }
    }

    /// <summary>
    /// Maps a [Entity] domain entity to [Entity]Dto for response.
    /// </summary>
    private static [Entity]Dto MapToDto([Entity] entity)
    {
        return new [Entity]Dto
        {
            Id = entity.Id,
            // CUSTOMIZE: Map entity properties to DTO
            // Code = entity.Code,
            // Name = entity.Name,
            CreatedAt = entity.Created
        };
    }

    // ICrudService interface implementation (for base interface compatibility)
    async Task<Result<[Entity]Dto?>> ICrudService<[Entity]Dto, Create[Entity]Dto, Update[Entity]Dto>.GetByIdAsync(
        Guid id, CancellationToken ct)
        => await Get[Entity]ByIdAsync(id, ct);

    async Task<Result<PaginatedList<[Entity]Dto>>> ICrudService<[Entity]Dto, Create[Entity]Dto, Update[Entity]Dto>.GetAllAsync(
        int pageNumber, int pageSize, CancellationToken ct)
        => await GetAll[Entities]Async(pageNumber, pageSize, ct);

    async Task<Result<[Entity]Dto>> ICrudService<[Entity]Dto, Create[Entity]Dto, Update[Entity]Dto>.CreateAsync(
        Create[Entity]Dto dto, CancellationToken ct)
        => await Create[Entity]Async(dto, ct);

    async Task<Result<[Entity]Dto>> ICrudService<[Entity]Dto, Create[Entity]Dto, Update[Entity]Dto>.UpdateAsync(
        Guid id, Update[Entity]Dto dto, CancellationToken ct)
        => await Update[Entity]Async(id, dto, ct);

    async Task<Result> ICrudService<[Entity]Dto, Create[Entity]Dto, Update[Entity]Dto>.DeleteAsync(
        Guid id, CancellationToken ct)
        => await Delete[Entity]Async(id, ct);
}
