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

        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Admin", "Staff", "HR" })
        {
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));
        }

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
                Currency = "VND"
            });
        }

        if(!await db.CategoryTypes.AnyAsync())
        {
            db.CategoryTypes.AddRange(
                new CategoryType { Code="DienTu" ,Name = "Điện tử" },
                new CategoryType { Code="ThoiTrang", Name = "Thời trang" },
                new CategoryType { Code="ThucPham", Name = "Thực phẩm" }
            );
        }
        await db.SaveChangesAsync();

        if (!await db.Categories.AnyAsync())
        {
            Guid DienTu= db.CategoryTypes.First(ct => ct.Code == "DienTu").Id;
            Guid ThoiTrang= db.CategoryTypes.First(ct => ct.Code == "ThoiTrang").Id;
            Guid ThucPham= db.CategoryTypes.First(ct => ct.Code == "ThucPham").Id;
            db.Categories.AddRange(
                new Category { Code = "LapTop", Name = "Laptop", CategoryTypeId = DienTu },
                new Category { Code = "PC", Name = "Máy tính để bàn", CategoryTypeId = DienTu },
                new Category { Code = "LapTopHp", Name = "Laptop Hp", CategoryTypeId = DienTu },
                new Category { Code = "Chuot", Name = "Chuột", CategoryTypeId = DienTu },
                new Category { Code = "BanPhim", Name = "Bàn phím", CategoryTypeId = DienTu },
                new Category { Code = "ManHinh", Name = "Màn hình", CategoryTypeId = DienTu },
                new Category { Code = "TaiNghe", Name = "Tai nghe", CategoryTypeId = DienTu },

                new Category { Code = "Ao", Name = "Áo", CategoryTypeId = ThoiTrang },
                new Category { Code = "Vay", Name = "Váy", CategoryTypeId = ThoiTrang },
                new Category { Code = "QuanJeans", Name = "Quần jeans", CategoryTypeId = ThoiTrang },
                new Category { Code = "MiAnLien", Name = "Mì ăn liền", CategoryTypeId = ThucPham },
                new Category { Code = "Sua", Name = "Sữa", CategoryTypeId = ThucPham }
            );
        }
        await db.SaveChangesAsync();

        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(
                new Product
                {
                    Sku = "SP-001",
                    Name = "Laptop văn phòng 14\"",
                    Unit = "Cái",
                    CostPrice = 12_500_000,
                    SalePrice = 15_900_000,
                    MinStock = 5,
                    CategoryId = db.Categories.First(c => c.Code == "LapTop").Id
                },
                new Product
                {
                    Sku = "SP-002",
                    Name = "Chuột không dây",
                    Unit = "Cái",
                    CostPrice = 120_000,
                    SalePrice = 199_000,
                    MinStock = 20,
                    CategoryId = db.Categories.First(c => c.Code == "Chuot").Id
                },
                new Product
                {
                    Sku = "SP-003",
                    Name = "Bàn phím cơ",
                    Unit = "Cái",
                    CostPrice = 650_000,
                    SalePrice = 990_000,
                    MinStock = 10,
                    CategoryId = db.Categories.First(c => c.Code == "BanPhim").Id
                },
                new Product
                {
                    Sku = "SP-004",
                    Name = "Màn hình 24\"",
                    Unit = "Cái",
                    CostPrice = 2_400_000,
                    SalePrice = 3_290_000,
                    MinStock = 4,
                    CategoryId = db.Categories.First(c => c.Code == "ManHinh").Id
                },
                new Product
                {
                    Sku = "SP-005",
                    Name = "Tai nghe USB",
                    Unit = "Cái",
                    CostPrice = 180_000,
                    SalePrice = 279_000,
                    MinStock = 8,
                    CategoryId = db.Categories.First(c => c.Code == "TaiNghe").Id
                }
            );
        }

        await EnsureWarehouse(db, "WH-HCM", "Kho Hồ Chí Minh", WarehouseType.Main, "VN", "TP. Hồ Chí Minh", "Quận 12", "Asia/Ho_Chi_Minh", isDefault: true);
        await EnsureWarehouse(db, "WH-HN", "Kho Hà Nội", WarehouseType.Branch, "VN", "Hà Nội", "Long Biên", "Asia/Ho_Chi_Minh", isDefault: false);
        await EnsureWarehouse(db, "WH-BKK", "Kho Bangkok", WarehouseType.Branch, "TH", "Bangkok", "Lat Krabang", "Asia/Bangkok", isDefault: false);
        await db.SaveChangesAsync();

        if (!await db.Partners.AnyAsync())
        {
            db.Partners.AddRange(
                new Partner { Code = "KH-001", Name = "Công ty ABC", Type = PartnerType.Customer, Phone = "0901111222", Email = "mua@abc.vn", TaxCode = "0301111111", Address = "Bình Thạnh, HCM" },
                new Partner { Code = "KH-002", Name = "Cửa hàng Minh An", Type = PartnerType.Customer, Phone = "0903333444", Email = "minhan@shop.vn", Address = "Hà Nội" },
                new Partner { Code = "NCC-01", Name = "Nhà cung cấp Delta", Type = PartnerType.Supplier, Phone = "0289999888", Email = "delta@ncc.vn", TaxCode = "0319999999", Address = "Thủ Đức, HCM" },
                new Partner { Code = "DT-01", Name = "Đối tác Hòa Bình", Type = PartnerType.Both, Phone = "0915555666", Email = "hb@doitac.vn", Address = "Đà Nẵng" }
            );
        }

        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Code = "PB-KD", Name = "Kinh doanh" },
                new Department { Code = "PB-NS", Name = "Nhân sự" },
                new Department { Code = "PB-KT", Name = "Kế toán" },
                new Department { Code = "PB-IT", Name = "Công nghệ thông tin" }
            );
        }

        if (!await db.JobPositions.AnyAsync())
        {
            db.JobPositions.AddRange(
                new JobPosition { Code = "CV-NV", Name = "Nhân viên" },
                new JobPosition { Code = "CV-TT", Name = "Trưởng nhóm" },
                new JobPosition { Code = "CV-TP", Name = "Trưởng phòng" },
                new JobPosition { Code = "CV-GD", Name = "Giám đốc" }
            );
        }

        await db.SaveChangesAsync();

        if (!await db.StockBalances.AnyAsync() && await db.Products.AnyAsync() && await db.Warehouses.AnyAsync())
        {
            var hcm = await db.Warehouses.FirstAsync(x => x.Code == "WH-HCM");
            var hn = await db.Warehouses.FirstAsync(x => x.Code == "WH-HN");
            var bkk = await db.Warehouses.FirstAsync(x => x.Code == "WH-BKK");
            var products = await db.Products.ToDictionaryAsync(x => x.Sku);

            void Bal(string sku, Warehouse wh, decimal qty) =>
                db.StockBalances.Add(new StockBalance { ProductId = products[sku].Id, WarehouseId = wh.Id, Qty = qty });

            Bal("SP-001", hcm, 12); Bal("SP-001", hn, 6);
            Bal("SP-002", hcm, 50); Bal("SP-002", hn, 20); Bal("SP-002", bkk, 10);
            Bal("SP-003", hcm, 20); Bal("SP-003", hn, 15);
            Bal("SP-004", hcm, 8); Bal("SP-004", bkk, 4);
            Bal("SP-005", hcm, 2); Bal("SP-005", hn, 1);
            await db.SaveChangesAsync();
        }

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

            var eAdmin = new Employee { Code = "NV-001", FullName = "Nguyễn Quản Trị", IdentityNumber= "NV-001-001", Email = "admin@erplite.local", Phone = "0900000001", DepartmentId = it.Id, PositionId = gd.Id, HireDate = new DateTime(2022, 1, 10), Gender = Gender.Male, Status = EmployeeStatus.Working, UserId = admin?.Id };
            var eSales = new Employee { Code = "NV-002", FullName = "Trần Bán Hàng", IdentityNumber = "NV-001-002", Email = "sales@erplite.local", Phone = "0900000002", DepartmentId = kd.Id, PositionId = nv.Id, HireDate = new DateTime(2024, 3, 1), Gender = Gender.Female, Status = EmployeeStatus.Working, UserId = sales?.Id };
            var eHr = new Employee { Code = "NV-003", FullName = "Lê Nhân Sự", IdentityNumber = "NV-001-003", Email = "hr@erplite.local", Phone = "0900000003", DepartmentId = ns.Id, PositionId = tp.Id, HireDate = new DateTime(2023, 6, 15), Gender = Gender.Female, Status = EmployeeStatus.Working, UserId = hr?.Id };
            var eNew = new Employee { Code = "NV-004", FullName = "Phạm Thử Việc", IdentityNumber = "NV-001-004", Email = "intern@erplite.local", Phone = "0900000004", DepartmentId = kd.Id, PositionId = nv.Id, HireDate = DateTime.Today.AddMonths(-1), Gender = Gender.Male, Status = EmployeeStatus.Probation };

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
                Status = LeaveStatus.Pending
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

    private static async Task EnsureWarehouse(AppDbContext db, string code, string name, WarehouseType type,
        string country, string city, string address, string tz, bool isDefault)
    {
        if (await db.Warehouses.AnyAsync(x => x.Code == code)) return;
        db.Warehouses.Add(new Warehouse
        {
            Code = code,
            Name = name,
            Type = type,
            CountryCode = country,
            City = city,
            Address = address,
            TimeZone = tz,
            IsDefault = isDefault,
            IsActive = true
        });
    }
}
