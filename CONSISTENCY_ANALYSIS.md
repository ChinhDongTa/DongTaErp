# Application Layer Consistency Analysis & Improvement Recommendations

## Executive Summary

Your Application layer has **significant inconsistencies** across service implementations. While recent work on Partner, Category, and CompanyProfile services established a solid improved pattern, many other services don't follow this standard. This analysis identifies the issues and proposes a unified architecture.

---

## 1. CRITICAL INCONSISTENCIES IDENTIFIED

### 1.1 Service Interface Patterns

**Current Chaos:**
- Some services use `ICrudService<TDto, TCreateDto, TUpdateDto>` base interface only
- Others add custom `SearchAsync()` methods with individual parameters
- One service (IEmployeeService) uses `SimpleSearch` object parameter
- Some have lookup/relationship methods; others don't

**Examples:**
| Service | Pattern | Status |
|---------|---------|--------|
| IPartnerService | ICrudService + SearchAsync(string?, int, int, string?, bool) | ✅ Modern |
| ICategoryService | ICrudService + SearchAsync(...) + GetTypesAsync() + GetParentsAsync() | Mixed (has extra methods not on others) |
| IDepartmentService | ICrudService only (no search) | ❌ Outdated |
| IEmployeeService | ICrudService + SearchAsync(SimpleSearch) | ⚠️ Different pattern |
| IProductService | ICrudService + SearchAsync(string?, int, int, string?, bool) | ✅ Modern |
| IWarehouseService | ICrudService + SearchAsync(...) + GetStockAsync() | ✅ Modern + domain-specific |

### 1.2 Service Implementation Patterns

**Inconsistencies in Constructor/DI:**
```csharp
// NEW MODERN PATTERN (Partner, Category, CompanyProfile):
public class PartnerService(IUnitOfWork unitOfWork, ILogger<PartnerService> logger) : IPartnerService
{ }

// OLD PATTERN (GenericCrudService-based, e.g., Departments, legacy services):
protected GenericCrudService(
	IRepository<TEntity> repository,
	IUnitOfWork unitOfWork,
	ILogger logger,
	IValidator<TCreateDto> createValidator,
	IValidator<TUpdateDto> updateValidator)
{ }

// MIXED PATTERN (Employees):
// Uses GenericCrudService but overrides behavior significantly
```

**Problem:** Services don't use the modern primary constructor DI pattern consistently.

### 1.3 Sorting/Pagination Implementation

**ApplySorting() Inconsistency:**

**Old Pattern (Partners before improvement):**
```csharp
public static IQueryable<Partner> ApplySorting(this IQueryable<Partner> query, string? sortBy = null, bool isDescending = false)
{
	return sortBy?.Trim().ToLowerInvariant() switch
	{
		"code" => isDescending ? query.OrderByDescending(p => p.Code) : query.OrderBy(p => p.Code),
		"name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
		_ => query.OrderByDescending(p => p.Created)
	};
}
```

**New IMPROVED Pattern (CompanyProfile):**
```csharp
public static IQueryable<CompanyProfile> ApplySorting(
	this IQueryable<CompanyProfile> query, string? sortBy = null, bool isDescending = false)
{
	return sortBy?.Trim().ToLowerInvariant() switch
	{
		"name" => OrderBy(query, c => c.Name, isDescending),
		"phone" => OrderBy(query, c => c.Phone, isDescending),
		_ => OrderBy(query, c => c.Created, isDescending)
	};
}

private static IOrderedQueryable<CompanyProfile> OrderBy<TKey>(
	IQueryable<CompanyProfile> query,
	Expression<Func<CompanyProfile, TKey>> key,
	bool isDescending)
{
	return isDescending ? query.OrderByDescending(key) : query.OrderBy(key);
}
```

**Benefit:** New pattern eliminates ternary duplication with reusable helper.

### 1.4 Search Method Pattern Inconsistency

**Inconsistent Parameter Approaches:**
```csharp
// STANDARD (Partner, Product, Warehouse, Category, CompanyProfile):
Task<Result<PaginatedList<TDto>>> SearchAsync(
	string? searchTerm, int pageNumber = 1, int pageSize = 20, 
	string? sortBy = null, bool isDescending = false, CancellationToken ct = default);

// OBJECT-BASED (Employee - single outlier):
Task<Result<PaginatedList<EmployeeDto>>> SearchAsync(SimpleSearch search, CancellationToken ct = default);

// MISSING (Departments & others):
// No SearchAsync at all
```

**Problem:** Employee/EmployeeService is the only service using an object parameter. This breaks consistency.

### 1.5 Logging Inconsistency

**Partner/Category/CompanyProfile (Modern):**
- Detailed method-level logging at entry/action points
- Exception logging with structured context
- Avoid exposing internal details in result messages

**GenericCrudService (Old):**
- Generic try/catch with `Logger.LogError(ex, "Error getting {Entity}", typeof(TEntity).Name)`
- Less specific to action

### 1.6 Update Logic Inconsistency

**Modern Pattern (Partner, Category, CompanyProfile):**
```csharp
if (dto.Code.HasValueAndIsDifferentFrom(partner.Code))
{
	partner.Code = dto.Code!;
}
```
Benefits: Only updates fields that actually changed; clearer intent.

**GenericCrudService Pattern:**
```csharp
entity = Mapper.Map(dto, entity);
```
Problems: All fields mapped regardless of whether they changed; loses change tracking intent.

### 1.7 Result Pattern Inconsistency

**Modern Services Return:**
- `Result<TDto>` for create/update operations
- `Result<PaginatedList<TDto>>` for list operations
- `Result` for delete operations
- Proper status codes (Created, Success, NotFound, NoContent, CriticalError)

**Some Services via GenericCrudService:**
- `CreateAsync` returns `Result<string>` (inconsistent!)
- Less consistent status handling

---

## 2. PATTERN RECOMMENDATIONS

### 2.1 Unified Service Interface Pattern

**Standard Interface Structure (Recommended):**
```csharp
public interface I[Entity]Service : ICrudService<[Entity]Dto, Create[Entity]Dto, Update[Entity]Dto>
{
	// SearchAsync is standard for all searchable entities
	Task<Result<PaginatedList<[Entity]Dto>>> SearchAsync(
		string? searchTerm = null,
		int pageNumber = 1,
		int pageSize = 20,
		string? sortBy = null,
		bool isDescending = false,
		CancellationToken ct = default);

	// Domain-specific lookup/relationship methods only if needed
	// Example: Product has unit category lookups
	// Task<Result<List<LookupItemDto>>> GetCategoriesAsync(CancellationToken ct = default);
}
```

**Exceptions:**
- Services with **no search requirement** (e.g., AnnouncementService, simple lookups): implement only `ICrudService`
- Services with **domain-specific queries** (e.g., Warehouse.GetStockAsync, Invoice.GetByPartner): add domain methods after SearchAsync

### 2.2 Unified Service Implementation Pattern

**Standard Constructor (Primary Constructor DI):**
```csharp
public class [Entity]Service(IUnitOfWork unitOfWork, ILogger<[Entity]Service> logger) : I[Entity]Service
{
	// Inject only what's needed; UnitOfWork provides access to all repositories
}
```

**Benefits:**
- ✅ Cleaner signatures
- ✅ Consistent across all services
- ✅ Easy to locate dependencies
- ✅ No unnecessary IValidator injection for standard CRUDs

**When to use GenericCrudService:**
- Only for simple list/lookups that need **no search, sorting, or custom logic**
- Example: static categorical data (GoodsReceiptStatus, LeaveRequestType)

### 2.3 Unified Extension Pattern (Sorting & Projection)

**Standard File Structure: `Services/[Entity]/Extensions.cs`**
```csharp
namespace DongTaErp.Application.Services.[Entity]s;

internal static class Extensions
{
	public static IQueryable<[Entity]> ApplySorting(
		this IQueryable<[Entity]> query,
		string? sortBy = null,
		bool isDescending = false)
	{
		return sortBy?.Trim().ToLowerInvariant() switch
		{
			"field1" => OrderBy(query, e => e.Field1, isDescending),
			"field2" => OrderBy(query, e => e.Field2, isDescending),
			_ => OrderBy(query, e => e.Created, isDescending) // Default: newest first
		};
	}

	private static IOrderedQueryable<[Entity]> OrderBy<TKey>(
		IQueryable<[Entity]> query,
		Expression<Func<[Entity], TKey>> key,
		bool isDescending)
	{
		return isDescending ? query.OrderByDescending(key) : query.OrderBy(key);
	}

	public static IQueryable<[Entity]Dto> To[Entity]Dto(this IQueryable<[Entity]> query)
	{
		return query.Select(e => new [Entity]Dto
		{
			Id = e.Id,
			// Map all DTO properties
			CreatedAt = e.Created
		});
	}
}
```

**Benefits:**
- Reusable `OrderBy<TKey>()` helper eliminates verbosity
- Consistent sorting defaults (Created descending)
- Projection always named `To[Entity]Dto()`

### 2.4 Unified CRUD Method Pattern

**GetByIdAsync:**
```csharp
public async Task<Result<[Entity]Dto?>> Get[Entity]ByIdAsync(Guid id, CancellationToken ct = default)
{
	var result = await unitOfWork.[Entities].Query()
		.Where(e => e.Id == id)
		.To[Entity]Dto()
		.SingleOrNotFoundAsync("[Entity]", id, ct);
	return result;
}
```

**GetAllAsync:**
```csharp
public async Task<Result<PaginatedList<[Entity]Dto>>> GetAll[Entities]Async(
	int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
{
	try
	{
		var paginated = await unitOfWork.[Entities].Query()
			.ApplySorting() // Default sorting
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
```

**SearchAsync (if needed):**
```csharp
public async Task<Result<PaginatedList<[Entity]Dto>>> Search[Entities]Async(
	string? searchTerm, int pageNumber = 1, int pageSize = 20,
	string? sortBy = null, bool isDescending = false, CancellationToken ct = default)
{
	try
	{
		var query = unitOfWork.[Entities].Query();

		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			var lowerSearchTerm = $"%{searchTerm.ToLower()}%";
			query = query.Where(e => 
				EF.Functions.Like(e.SearchableField1.ToLower(), lowerSearchTerm) ||
				EF.Functions.Like(e.SearchableField2.ToLower(), lowerSearchTerm));
		}

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
```

**CreateAsync:**
```csharp
public async Task<Result<[Entity]Dto>> Create[Entity]Async(Create[Entity]Dto dto, CancellationToken ct = default)
{
	try
	{
		var entity = new [Entity]
		{
			// Map DTO fields
		};

		await unitOfWork.[Entities].AddAsync(entity, ct);
		await unitOfWork.SaveChangesAsync(ct);

		return Result<[Entity]Dto>.Created(MapToDto(entity));
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "An error occurred while creating a new [entity].");
		return Result<[Entity]Dto>.CriticalError("An error occurred while creating the [entity].");
	}
}
```

**UpdateAsync:**
```csharp
public async Task<Result<[Entity]Dto>> Update[Entity]Async(Guid id, Update[Entity]Dto dto, CancellationToken ct = default)
{
	try
	{
		var entity = await unitOfWork.[Entities].GetByIdAsync(id, ct);
		if (entity == null)
			return Result<[Entity]Dto>.NotFound(ErrorHelpers.NotFoundWithId("[Entity]", id));

		// Use patch-style updates: only update if field changed
		if (dto.Field1.HasValueAndIsDifferentFrom(entity.Field1))
			entity.Field1 = dto.Field1!;

		if (dto.Field2.IsDifferentFrom(entity.Field2))
			entity.Field2 = dto.Field2;

		await unitOfWork.SaveChangesAsync(ct);
		return Result<[Entity]Dto>.Success(MapToDto(entity));
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "An error occurred while updating the [entity].");
		return Result<[Entity]Dto>.CriticalError("An error occurred while updating the [entity].");
	}
}
```

**DeleteAsync:**
```csharp
public async Task<Result> Delete[Entity]Async(Guid id, CancellationToken ct = default)
{
	try
	{
		var entity = await unitOfWork.[Entities].GetByIdAsync(id, ct);
		if (entity == null)
			return Result.NotFound(ErrorHelpers.NotFoundWithId("[Entity]", id));

		unitOfWork.[Entities].Delete(entity);
		await unitOfWork.SaveChangesAsync(ct);

		return Result.Success(ResultStatus.NoContent);
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "An error occurred while deleting the [entity].");
		return Result.Error("An error occurred while deleting the [entity].");
	}
}
```

### 2.5 Recommendations for ICrudService Interface

**Current Issue:**
- Base `ICrudService` defines methods but `CreateAsync` returns `Result<string>` (inconsistent!)
- `GetUpdateByIdAsync` is rarely used

**Proposed Change:**
```csharp
public interface ICrudService<TDto, TCreateDto, TUpdateDto>
{
	Task<Result<TDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
	Task<Result<PaginatedList<TDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
	Task<Result<TDto>> CreateAsync(TCreateDto dto, CancellationToken ct = default); // Changed from Result<string>
	Task<Result<TDto>> UpdateAsync(Guid id, TUpdateDto dto, CancellationToken ct = default); // Added return type
	Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);

	// Optional: Keep for special cases where you need update form data
	// Task<Result<TUpdateDto?>> GetUpdateByIdAsync(Guid id, CancellationToken ct = default);
}
```

---

## 3. PRIORITY REMEDIATION ROADMAP

### Phase 1: Define Standards (Week 1)
- ✅ Document unified patterns (this document)
- Create template service files for copy-paste consistency
- Update `GlobalUsing.cs` to include all common extensions

### Phase 2: High-Value Services (Week 2-3)
Priority order based on **frequency of use + inconsistency level**:

1. **Employee** (high inconsistency: uses SimpleSearch)
   - Migrate to individual search parameters
   - Add logging consistency

2. **Product** (mostly modern, needs validation)
   - Ensure Extensions.cs uses `OrderBy<TKey>()` helper
   - Validate search fields match domain entity

3. **Warehouse** (mostly modern, needs validation)
   - Ensure consistent with Product pattern
   - Validate stock-specific methods follow Result pattern

4. **Department** (missing search functionality)
   - Add SearchAsync consistently
   - Add Extensions.cs with ApplySorting/ToDto

5. **Payments** (likely inconsistent)
   - Migrate to modern pattern
   - Validate payment-specific logic

6. **Payroll** (likely inconsistent)
   - Migrate to modern pattern
   - Validate domain-specific methods

### Phase 3: Transaction Services (Week 4)
- GoodsReceipt, GoodsIssue, Invoice, VendorBill
- These are likely complex; migrate carefully

### Phase 4: Remove GenericCrudService (Week 5)
- Only applicable to truly simple entities (lookup tables)
- Migrate remaining services to primary constructor DI
- Clean up old pattern artifacts

### Phase 5: Update DI Registration (Week 5)
- Verify all services registered with correct interfaces
- Remove any GenericCrudService registrations
- Update `DependencyInjection.cs` comments with pattern reference

---

## 4. QUICK WINS (Can Start Immediately)

1. **Create Service Template File**
   - File: `TEMPLATE_Service.cs` in `Services/_ServiceTemplate/`
   - Include all standard methods with inline comments
   - Document what to customize per entity

2. **Standardize Extensions.cs Pattern**
   - All Extensions.cs files use reusable `OrderBy<TKey>()` helper
   - Document expected searchable fields in comments
   - Add TODO comments for custom sorting if needed

3. **Audit DTO Update Types**
   - Ensure all `UpdateXxxDto` have nullable fields for patch-style updates
   - Add `#nullable enable` at file top if needed
   - Document which fields are optional vs. required

4. **Add XML Documentation Comments**
   - Service interfaces: summarize each method's behavior
   - Extensions: document sortable fields
   - Example: `/// <param name="sortBy">Supported values: "code", "name", "phone", "email". Default: creation date descending.</param>`

---

## 5. TECHNICAL DEBT & RISKS

| Issue | Impact | Recommendation |
|-------|--------|-----------------|
| GenericCrudService still used | Hard to debug; mixes old/new patterns | Deprecate and migrate during Phase 4 |
| SimpleSearch parameter (Employee) | Breaking consistency | Migrate to individual params |
| Result<string> from CreateAsync | Caller confusion (what string?) | Change to Result<TDto> |
| Missing SearchAsync on some entities | Users can't find data | Add to all searchable entities |
| Inconsistent logging levels | Debugging difficulty | Standardize on logger.LogError for errors |
| No update comparison logic in old services | All fields updated even if unchanged | Implement HasValueAndIsDifferentFrom pattern |
| Validators injected but unused in new pattern | Code smell | Remove from modern services, decide GenericCrudService approach |

---

## 6. EXAMPLE MIGRATION: DepartmentService

**Current (Old):**
```csharp
public class DepartmentService(
	IRepository<Department> repository,
	IUnitOfWork unitOfWork,
	ILogger<DepartmentService> logger,
	IValidator<CreateDepartmentDto> createValidator,
	IValidator<UpdateDepartmentDto> updateValidator)
	: GenericCrudService<Department, DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>(
		repository, unitOfWork, logger, createValidator, updateValidator), IDepartmentService
{
}
```

**Status:** No SearchAsync, uses GenericCrudService.

**Target (New):**
```csharp
using Microsoft.Extensions.Logging;

namespace DongTaErp.Application.Services.Departments;

public class DepartmentService(IUnitOfWork unitOfWork, ILogger<DepartmentService> logger) : IDepartmentService
{
	public async Task<Result<DepartmentDto?>> GetDepartmentByIdAsync(Guid id, CancellationToken ct = default)
	{
		var result = await unitOfWork.Departments.Query()
			.Where(d => d.Id == id)
			.ToDepartmentDto()
			.SingleOrNotFoundAsync("Department", id, ct);
		return result;
	}

	public async Task<Result<PaginatedList<DepartmentDto>>> GetAllDepartmentsAsync(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
	{
		try
		{
			var paginated = await unitOfWork.Departments.Query()
				.ApplySorting()
				.ToDepartmentDto()
				.PaginatedListAsync(pageNumber, pageSize, ct);

			return Result<PaginatedList<DepartmentDto>>.Success(paginated);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while fetching departments.");
			return Result<PaginatedList<DepartmentDto>>.CriticalError("Internal server error occurred while fetching departments.");
		}
	}

	public async Task<Result<PaginatedList<DepartmentDto>>> SearchDepartmentsAsync(string? searchTerm, int pageNumber = 1, int pageSize = 20, string? sortBy = null, bool isDescending = false, CancellationToken ct = default)
	{
		try
		{
			var query = unitOfWork.Departments.Query();

			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				var lowerSearchTerm = $"%{searchTerm.ToLower()}%";
				query = query.Where(d => EF.Functions.Like(d.Name.ToLower(), lowerSearchTerm));
			}

			var paginatedList = await query
				.ApplySorting(sortBy, isDescending)
				.ToDepartmentDto()
				.PaginatedListAsync(pageNumber, pageSize, ct);

			return Result<PaginatedList<DepartmentDto>>.Success(paginatedList);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while searching departments.");
			return Result<PaginatedList<DepartmentDto>>.CriticalError("Internal server error occurred while searching departments.");
		}
	}

	public async Task<Result<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto dto, CancellationToken ct = default)
	{
		try
		{
			var department = new Department
			{
				Name = dto.Name,
				Description = dto.Description
			};

			await unitOfWork.Departments.AddAsync(department, ct);
			await unitOfWork.SaveChangesAsync(ct);

			return Result<DepartmentDto>.Created(MapToDto(department));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while creating a new department.");
			return Result<DepartmentDto>.CriticalError("An error occurred while creating the department.");
		}
	}

	public async Task<Result<DepartmentDto>> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto dto, CancellationToken ct = default)
	{
		try
		{
			var department = await unitOfWork.Departments.GetByIdAsync(id, ct);
			if (department == null)
				return Result<DepartmentDto>.NotFound(ErrorHelpers.NotFoundWithId("Department", id));

			if (dto.Name.HasValueAndIsDifferentFrom(department.Name))
				department.Name = dto.Name!;

			if (dto.Description.IsDifferentFrom(department.Description))
				department.Description = dto.Description;

			await unitOfWork.SaveChangesAsync(ct);
			return Result<DepartmentDto>.Success(MapToDto(department));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while updating the department.");
			return Result<DepartmentDto>.CriticalError("An error occurred while updating the department.");
		}
	}

	public async Task<Result> DeleteDepartmentAsync(Guid id, CancellationToken ct = default)
	{
		try
		{
			var department = await unitOfWork.Departments.GetByIdAsync(id, ct);
			if (department == null)
				return Result.NotFound(ErrorHelpers.NotFoundWithId("Department", id));

			unitOfWork.Departments.Delete(department);
			await unitOfWork.SaveChangesAsync(ct);

			return Result.Success(ResultStatus.NoContent);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while deleting the department.");
			return Result.Error("An error occurred while deleting the department.");
		}
	}

	private static DepartmentDto MapToDto(Department department)
	{
		return new DepartmentDto
		{
			Id = department.Id,
			Name = department.Name,
			Description = department.Description,
			CreatedAt = department.Created
		};
	}

	// Implement base interface compliance if needed
	async Task<Result<DepartmentDto?>> ICrudService<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>.GetByIdAsync(Guid id, CancellationToken ct)
		=> await GetDepartmentByIdAsync(id, ct);

	async Task<Result<PaginatedList<DepartmentDto>>> ICrudService<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>.GetAllAsync(int pageNumber, int pageSize, CancellationToken ct)
		=> await GetAllDepartmentsAsync(pageNumber, pageSize, ct);

	async Task<Result<DepartmentDto>> ICrudService<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>.CreateAsync(CreateDepartmentDto dto, CancellationToken ct)
		=> await CreateDepartmentAsync(dto, ct);

	async Task<Result<DepartmentDto>> ICrudService<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>.UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken ct)
		=> await UpdateDepartmentAsync(id, dto, ct);

	async Task<Result> ICrudService<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>.DeleteAsync(Guid id, CancellationToken ct)
		=> await DeleteDepartmentAsync(id, ct);
}
```

**Also Create: `Services/Departments/Extensions.cs`**
```csharp
using System.Linq.Expressions;

namespace DongTaErp.Application.Services.Departments;

internal static class Extensions
{
	public static IQueryable<Department> ApplySorting(this IQueryable<Department> query, string? sortBy = null, bool isDescending = false)
	{
		return sortBy?.Trim().ToLowerInvariant() switch
		{
			"name" => OrderBy(query, d => d.Name, isDescending),
			_ => OrderBy(query, d => d.Created, isDescending)
		};
	}

	private static IOrderedQueryable<Department> OrderBy<TKey>(
		IQueryable<Department> query,
		Expression<Func<Department, TKey>> key,
		bool isDescending)
	{
		return isDescending ? query.OrderByDescending(key) : query.OrderBy(key);
	}

	public static IQueryable<DepartmentDto> ToDepartmentDto(this IQueryable<Department> query)
	{
		return query.Select(d => new DepartmentDto
		{
			Id = d.Id,
			Name = d.Name,
			Description = d.Description,
			CreatedAt = d.Created
		});
	}
}
```

**Update: `Services/Departments/IDepartmentService.cs`**
```csharp
namespace DongTaErp.Application.Services.Departments;

public interface IDepartmentService : ICrudService<DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto>
{
	Task<Result<PaginatedList<DepartmentDto>>> SearchDepartmentsAsync(
		string? searchTerm,
		int pageNumber = 1,
		int pageSize = 20,
		string? sortBy = null,
		bool isDescending = false,
		CancellationToken ct = default);
}
```

---

## Summary: Key Takeaways

✅ **Adopt the Partner/Category/CompanyProfile pattern** as your standard  
✅ **Use primary constructor DI** exclusively  
✅ **Implement SearchAsync consistently** with individual string/int parameters, never objects  
✅ **Create Extensions.cs** in every service folder with ApplySorting & ToDto  
✅ **Use patch-style updates** with HasValueAndIsDifferentFrom/IsDifferentFrom  
✅ **Return Result<TDto>** from CreateAsync/UpdateAsync, not Result<string>  
✅ **Standardize error handling** with logger.LogError and CriticalError for exceptions  
✅ **Deprecate GenericCrudService** once all services are migrated  

**Estimated Effort:** 2-3 weeks for full remediation across ~40 services  
**Benefit:** Maintainability, consistency, debuggability, and adherence to Clean Architecture
