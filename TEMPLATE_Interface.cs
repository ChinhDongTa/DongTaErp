// TEMPLATE: Copy this file to Services/[Entity]/I[Entity]Service.cs

namespace DongTaErp.Application.Services.[Entities];

/// <summary>
/// Service interface for [Entity] CRUD and search operations.
/// 
/// Inherits standard CRUD methods from ICrudService base interface:
/// - GetByIdAsync: Get single [entity] by ID
/// - GetAllAsync: Get paginated list of all [entities]
/// - CreateAsync: Create new [entity]
/// - UpdateAsync: Update existing [entity]
/// - DeleteAsync: Delete [entity] by ID
/// 
/// Additional methods:
/// - SearchAsync: Search [entities] with filtering and sorting
/// 
/// TODO: Add domain-specific methods after SearchAsync if needed
/// Examples:
/// - Task<Result<List<[Entity]Dto>>> GetBy[RelatedEntity]Async(Guid [relatedEntity]Id, CancellationToken ct = default);
/// - Task<Result<[Entity]Dto>> Get[Special]Async(Guid id, CancellationToken ct = default);
/// </summary>
public interface I[Entity]Service : ICrudService<[Entity]Dto, Create[Entity]Dto, Update[Entity]Dto>
{
    /// <summary>
    /// Searches [entities] by term and applies optional sorting.
    /// </summary>
    /// <param name="searchTerm">Search term. Searches in: [CUSTOMIZE: e.g., "code", "name", "phone", "email"]. 
    /// Null or empty skips search filtering.</param>
    /// <param name="pageNumber">1-based page number; default is 1</param>
    /// <param name="pageSize">Number of records per page; default is 20</param>
    /// <param name="sortBy">Field name to sort by. Defaults to creation date descending.
    /// Supported values: [CUSTOMIZE: e.g., "code", "name", "phone", "email"]</param>
    /// <param name="isDescending">If true, sort descending; else ascending</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Success with paginated search results, or CriticalError if unexpected exception occurs</returns>
    Task<Result<PaginatedList<[Entity]Dto>>> Search[Entities]Async(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default);

    // TODO: Add domain-specific methods here if needed
    // Example: Task<Result<List<[Entity]Dto>>> GetBy[RelatedEntity]Async(Guid [relatedEntity]Id, CancellationToken ct = default);
}
