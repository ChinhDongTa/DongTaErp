namespace DongTaErp.Application.Common.Interfaces;

public interface ICrudService< TDto, TCreateDto, TUpdateDto> 
{
    Task<Result<TDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<TUpdateDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);

    Task<Result<PaginatedList<TDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);

    Task<Result<string>> CreateAsync(TCreateDto dto, CancellationToken ct = default);

    Task<Result> UpdateAsync(Guid id, TUpdateDto dto, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);

    Task<Result> SoftDeleteAsync(Guid id, CancellationToken ct = default);
}