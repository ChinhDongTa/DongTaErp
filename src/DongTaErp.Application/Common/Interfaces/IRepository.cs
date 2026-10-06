namespace DongTaErp.Application.Common.Interfaces;

/// <summary>
/// Generic repository interface cho base CRUD operations
/// </summary>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    // Create
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    // Read
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<IList<TEntity>> GetListAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
    IQueryable<TEntity> Query();

    // Update
    TEntity Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);

    // Delete
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);

    // Count
    Task<int> CountAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

    // Exists
    Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
}
