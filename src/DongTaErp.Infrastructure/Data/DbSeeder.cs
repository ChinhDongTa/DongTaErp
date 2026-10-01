using DongTaErp.Domain.Entities;
using DongTaErp.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
namespace DongTaErp.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        //await db.Database.MigrateAsync();

        //var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        //foreach (var role in new[] { "Admin", "Staff", "HR" })
        //{
        //    if (!await roles.RoleExistsAsync(role))
        //        await roles.CreateAsync(new IdentityRole(role));
        //}

        var users = services.GetRequiredService<UserManager<AppUser>>();
        //await EnsureUser(users, "admin", "Quản trị viên", "admin@erplite.local", "Admin@123", "Admin");
        //await EnsureUser(users, "sales", "Nhân viên bán hàng", "sales@erplite.local", "Sales@123", "Staff");
        //await EnsureUser(users, "hr", "Trưởng phòng nhân sự", "hr@erplite.local", "Hr@12345", "HR");

        if (!await db.CompanyProfiles.AnyAsync())
        {
            db.CompanyProfiles.Add(new CompanyProfile
            {
                Name = "ERP Lite Demo Co.",
                TaxCode = "0312345678",
                Phone = "028 1234 5678",
                Email = "hello@erplite.local",
                Address = "Quận 1, TP. Hồ Chí Minh",
                Currency = "VND",
                CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c",
                LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c"
            });
        }

        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(
                new Product { Sku = "SP-001", Name = "Laptop văn phòng 14\"", Unit = "Cái", CostPrice = 12_500_000, SalePrice = 15_900_000, StockQty = 18, MinStock = 5, CreatedBy= "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" ,LastModifiedBy= "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Product { Sku = "SP-002", Name = "Chuột không dây", Unit = "Cái", CostPrice = 120_000, SalePrice = 199_000, StockQty = 80, MinStock = 20, CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Product { Sku = "SP-003", Name = "Bàn phím cơ", Unit = "Cái", CostPrice = 650_000, SalePrice = 990_000, StockQty = 35, MinStock = 10, CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Product { Sku = "SP-004", Name = "Màn hình 24\"", Unit = "Cái", CostPrice = 2_400_000, SalePrice = 3_290_000, StockQty = 12, MinStock = 4, CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Product { Sku = "SP-005", Name = "Tai nghe USB", Unit = "Cái", CostPrice = 180_000, SalePrice = 279_000, StockQty = 3, MinStock = 8, CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" }
            );
        }

        if (!await db.Partners.AnyAsync())
        {
            db.Partners.AddRange(
                new Partner { Code = "KH-001", Name = "Công ty ABC", Type = PartnerType.Customer, Phone = "0901111222", Email = "mua@abc.vn", TaxCode = "0301111111", Address = "Bình Thạnh, HCM", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Partner { Code = "KH-002", Name = "Cửa hàng Minh An", Type = PartnerType.Customer, Phone = "0903333444", Email = "minhan@shop.vn", Address = "Hà Nội", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Partner { Code = "NCC-01", Name = "Nhà cung cấp Delta", Type = PartnerType.Supplier, Phone = "0289999888", Email = "delta@ncc.vn", TaxCode = "0319999999", Address = "Thủ Đức, HCM", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Partner { Code = "DT-01", Name = "Đối tác Hòa Bình", Type = PartnerType.Both, Phone = "0915555666", Email = "hb@doitac.vn", Address = "Đà Nẵng", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" }
            );
        }

        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Code = "PB-KD", Name = "Kinh doanh", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Department { Code = "PB-NS", Name = "Nhân sự", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Department { Code = "PB-KT", Name = "Kế toán", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new Department { Code = "PB-IT", Name = "Công nghệ thông tin", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" }
            );
        }

        if (!await db.JobPositions.AnyAsync())
        {
            db.JobPositions.AddRange(
                new JobPosition { Code = "CV-NV", Name = "Nhân viên", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new JobPosition { Code = "CV-TT", Name = "Trưởng nhóm", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new JobPosition { Code = "CV-TP", Name = "Trưởng phòng", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" },
                new JobPosition { Code = "CV-GD", Name = "Giám đốc", CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" }
            );
        }

        await db.SaveChangesAsync();

        if (!await db.Employees.AnyAsync())
        {
            var kd = await db.Departments.FirstAsync(x => x.Code == "PB-KD");
            var ns = await db.Departments.FirstAsync(x => x.Code == "PB-NS");
            var it = await db.Departments.FirstAsync(x => x.Code == "PB-IT");
            var nv = await db.JobPositions.FirstAsync(x => x.Code == "CV-NV");
            var tp = await db.JobPositions.FirstAsync(x => x.Code == "CV-TP");
            var gd = await db.JobPositions.FirstAsync(x => x.Code == "CV-GD");

            var admin = await users.FindByNameAsync("admin");
            var sales = await users.FindByNameAsync("sales");
            var hr = await users.FindByNameAsync("hr");

            var eAdmin = new Employee { Code = "NV-001", FullName = "Nguyễn Quản Trị", Email = "admin@erplite.local", Phone = "0900000001", DepartmentId = it.Id, PositionId = gd.Id, HireDate = new DateTime(2022, 1, 10), Gender = Gender.Male, Status = EmployeeStatus.Working, UserId = admin?.Id, CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" };
            var eSales = new Employee { Code = "NV-002", FullName = "Trần Bán Hàng", Email = "sales@erplite.local", Phone = "0900000002", DepartmentId = kd.Id, PositionId = nv.Id, HireDate = new DateTime(2024, 3, 1), Gender = Gender.Female, Status = EmployeeStatus.Working, UserId = sales?.Id, CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" };
            var eHr = new Employee { Code = "NV-003", FullName = "Lê Nhân Sự", Email = "hr@erplite.local", Phone = "0900000003", DepartmentId = ns.Id, PositionId = tp.Id, HireDate = new DateTime(2023, 6, 15), Gender = Gender.Female, Status = EmployeeStatus.Working, UserId = hr?.Id, CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" };
            var eNew = new Employee { Code = "NV-004", FullName = "Phạm Thử Việc", Email = "intern@erplite.local", Phone = "0900000004", DepartmentId = kd.Id, PositionId = nv.Id, HireDate = DateTime.Today.AddMonths(-1), Gender = Gender.Male, Status = EmployeeStatus.Probation, CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c", LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c" };

            db.Employees.AddRange(eAdmin, eSales, eHr, eNew);
            await db.SaveChangesAsync();

            db.LeaveRequests.Add(new LeaveRequest
            {
                EmployeeId = eSales.Id,
                Type = LeaveType.Annual,
                FromDate = DateTime.Today.AddDays(3),
                ToDate = DateTime.Today.AddDays(4),
                Days = 2,
                Reason = "Việc gia đình",
                Status = LeaveStatus.Pending,
                CreatedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c",
                LastModifiedBy = "2237df2f-20b7-4e1a-9dd1-7a6e0d95bc4c"
            });
            await db.SaveChangesAsync();
        }
    }

    private static async Task EnsureUser(UserManager<AppUser> users, string name, string display, string email, string password, string role)
    {
        var user = await users.FindByNameAsync(name);
        if (user is null)
        {
            user = new AppUser
            {
                UserName = name,
                Email = email,
                EmailConfirmed = true,
                DisplayName = display,
                IsActive = true
            };
            var result = await users.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        if (!await users.IsInRoleAsync(user, role))
            await users.AddToRoleAsync(user, role);
    }
}
