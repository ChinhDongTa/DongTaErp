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

        await SeedExtraProductsAsync(db);
        await db.SaveChangesAsync();

        await EnsureWarehouse(db, "WH-HCM", "Kho Hồ Chí Minh", WarehouseType.Main, "VN", "TP. Hồ Chí Minh", "Quận 12", "Asia/Ho_Chi_Minh", isDefault: true);
        await EnsureWarehouse(db, "WH-HN", "Kho Hà Nội", WarehouseType.Branch, "VN", "Hà Nội", "Long Biên", "Asia/Ho_Chi_Minh", isDefault: false);
        await EnsureWarehouse(db, "WH-DN", "Kho Đà Nẵng", WarehouseType.Transit, "VN", "Đà Nẵng", "Liên Chiểu", "Asia/Ho_Chi_Minh", isDefault: false);
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

        await SeedOpeningStockAsync(db);
        await SeedCommercialAsync(db);
        await SeedInboundAsync(db);
    }

    private static async Task SeedExtraProductsAsync(AppDbContext db)
    {
        var categories = await db.Categories.ToDictionaryAsync(x => x.Code);
        var existing = await db.Products.Select(x => x.Sku).ToListAsync();
        var known = existing.ToHashSet();

        (string Sku, string Name, string Unit, decimal Cost, decimal Sale, decimal Min, string Category)[] items =
        [
            ("SP-006", "Laptop gaming 15\"", "Cái", 18_500_000, 22_900_000, 3, "LapTop"),
            ("SP-007", "Laptop mỏng 13\"", "Cái", 16_200_000, 19_500_000, 3, "LapTop"),
            ("SP-008", "Laptop HP 14\"", "Cái", 11_800_000, 14_200_000, 4, "LapTopHp"),
            ("SP-009", "Laptop HP 15\"", "Cái", 13_400_000, 16_100_000, 4, "LapTopHp"),
            ("SP-010", "PC văn phòng i5", "Bộ", 9_800_000, 12_400_000, 2, "PC"),
            ("SP-011", "PC gaming RTX", "Bộ", 24_000_000, 28_900_000, 2, "PC"),
            ("SP-012", "Mini PC", "Cái", 7_200_000, 8_900_000, 3, "PC"),
            ("SP-013", "Chuột gaming", "Cái", 280_000, 420_000, 15, "Chuot"),
            ("SP-014", "Chuột văn phòng", "Cái", 90_000, 149_000, 25, "Chuot"),
            ("SP-015", "Chuột dọc", "Cái", 350_000, 520_000, 8, "Chuot"),
            ("SP-016", "Bàn phím không dây", "Cái", 420_000, 650_000, 10, "BanPhim"),
            ("SP-017", "Bàn phím số", "Cái", 180_000, 290_000, 8, "BanPhim"),
            ("SP-018", "Bàn phím low-profile", "Cái", 890_000, 1_250_000, 6, "BanPhim"),
            ("SP-019", "Màn hình 27\"", "Cái", 3_600_000, 4_790_000, 3, "ManHinh"),
            ("SP-020", "Màn hình 32\"", "Cái", 5_200_000, 6_890_000, 2, "ManHinh"),
            ("SP-021", "Màn hình di động 15\"", "Cái", 2_100_000, 2_790_000, 4, "ManHinh"),
            ("SP-022", "Tai nghe Bluetooth", "Cái", 450_000, 690_000, 10, "TaiNghe"),
            ("SP-023", "Tai nghe chụp tai", "Cái", 620_000, 890_000, 6, "TaiNghe"),
            ("SP-024", "Tai nghe gaming", "Cái", 780_000, 1_090_000, 6, "TaiNghe"),
            ("SP-025", "Webcam Full HD", "Cái", 390_000, 590_000, 8, "PC"),
            ("SP-026", "USB-C hub 6 cổng", "Cái", 250_000, 390_000, 12, "PC"),
            ("SP-027", "Ổ cứng SSD 512GB", "Cái", 890_000, 1_190_000, 10, "PC"),
            ("SP-028", "Ổ cứng SSD 1TB", "Cái", 1_450_000, 1_890_000, 8, "PC"),
            ("SP-029", "RAM 16GB", "Thanh", 720_000, 980_000, 10, "PC"),
            ("SP-030", "RAM 32GB", "Thanh", 1_350_000, 1_790_000, 6, "PC"),
            ("SP-031", "Áo thun nam", "Cái", 85_000, 159_000, 30, "Ao"),
            ("SP-032", "Áo sơ mi nữ", "Cái", 140_000, 259_000, 20, "Ao"),
            ("SP-033", "Áo khoác gió", "Cái", 220_000, 399_000, 12, "Ao"),
            ("SP-034", "Áo polo", "Cái", 110_000, 199_000, 20, "Ao"),
            ("SP-035", "Váy công sở", "Cái", 180_000, 329_000, 10, "Vay"),
            ("SP-036", "Váy dạ hội", "Cái", 450_000, 790_000, 4, "Vay"),
            ("SP-037", "Váy jean", "Cái", 190_000, 349_000, 8, "Vay"),
            ("SP-038", "Quần jeans nam", "Cái", 210_000, 389_000, 15, "QuanJeans"),
            ("SP-039", "Quần jeans nữ", "Cái", 200_000, 369_000, 15, "QuanJeans"),
            ("SP-040", "Quần jeans ống rộng", "Cái", 230_000, 419_000, 10, "QuanJeans"),
            ("SP-041", "Mì tôm chua cay", "Thùng", 95_000, 129_000, 20, "MiAnLien"),
            ("SP-042", "Mì ly bò", "Thùng", 110_000, 149_000, 20, "MiAnLien"),
            ("SP-043", "Mì chay", "Thùng", 88_000, 119_000, 15, "MiAnLien"),
            ("SP-044", "Sữa tươi 1L", "Lốc", 72_000, 98_000, 25, "Sua"),
            ("SP-045", "Sữa chua uống", "Lốc", 36_000, 52_000, 30, "Sua"),
            ("SP-046", "Sữa hạt", "Lốc", 48_000, 69_000, 20, "Sua"),
            ("SP-047", "Balo laptop", "Cái", 180_000, 299_000, 10, "LapTop"),
            ("SP-048", "Túi chống sốc 14\"", "Cái", 65_000, 119_000, 15, "LapTop"),
            ("SP-049", "Giá đỡ laptop", "Cái", 95_000, 169_000, 10, "LapTop"),
            ("SP-050", "Bàn di chuột lớn", "Cái", 70_000, 129_000, 12, "Chuot"),
            ("SP-051", "Dây nguồn PC", "Cái", 35_000, 59_000, 20, "PC"),
            ("SP-052", "Adapter 65W", "Cái", 210_000, 329_000, 8, "LapTop"),
            ("SP-053", "Cáp HDMI 2m", "Cái", 45_000, 79_000, 20, "ManHinh"),
            ("SP-054", "Loa máy tính", "Đôi", 160_000, 259_000, 8, "TaiNghe"),
            ("SP-055", "Microphone USB", "Cái", 320_000, 490_000, 6, "TaiNghe")
        ];

        foreach (var item in items)
        {
            if (known.Contains(item.Sku) || !categories.TryGetValue(item.Category, out var category))
                continue;

            db.Products.Add(new Product
            {
                Sku = item.Sku,
                Name = item.Name,
                Unit = item.Unit,
                CostPrice = item.Cost,
                SalePrice = item.Sale,
                MinStock = item.Min,
                CategoryId = category.Id,
                IsActive = true
            });
        }
    }

    private static async Task SeedInboundAsync(AppDbContext db)
    {
        if (await db.GoodsReceipts.AnyAsync(x => x.Code == "PN-002"))
            return;
        if (!await db.Partners.AnyAsync(x => x.Code == "NCC-01"))
            return;

        var supplier = await db.Partners.FirstAsync(x => x.Code == "NCC-01");
        var hcm = await db.Warehouses.FirstAsync(x => x.Code == "WH-HCM");
        var hn = await db.Warehouses.FirstAsync(x => x.Code == "WH-HN");
        var products = await db.Products
            .Where(x => x.Sku == "SP-013" || x.Sku == "SP-019" || x.Sku == "SP-027" || x.Sku == "SP-031" || x.Sku == "SP-044")
            .ToDictionaryAsync(x => x.Sku);
        if (products.Count < 5)
            return;

        PurchaseOrderLine Line(string sku, decimal qty)
        {
            var product = products[sku];
            return new PurchaseOrderLine
            {
                ProductId = product.Id,
                Qty = qty,
                UnitPrice = product.CostPrice,
                LineTotal = qty * product.CostPrice
            };
        }

        var lines = new List<PurchaseOrderLine>
        {
            Line("SP-013", 40),
            Line("SP-019", 8),
            Line("SP-027", 20),
            Line("SP-031", 60),
            Line("SP-044", 50)
        };
        var order = new PurchaseOrder
        {
            Number = "PO-002",
            PartnerId = supplier.Id,
            WarehouseId = hcm.Id,
            OrderDate = DateTime.Today.AddDays(-4),
            Status = PurchaseOrderStatus.Confirmed,
            ConfirmedAt = DateTime.UtcNow.AddDays(-4),
            TaxRate = 0.08m,
            Notes = "Đơn mua mẫu cho phiếu nhập nhiều kho.",
            Lines = lines
        };
        order.SubTotal = lines.Sum(x => x.LineTotal);
        order.TaxAmount = Math.Round(order.SubTotal * order.TaxRate, 2);
        order.Total = order.SubTotal + order.TaxAmount;
        db.PurchaseOrders.Add(order);
        await db.SaveChangesAsync();

        async Task Receive(string code, Warehouse warehouse, DateTime date, GoodsReceiptStatus status, params (string Sku, decimal Qty)[] qty)
        {
            var receiptLines = qty.Select(x => new GoodsReceiptLine
            {
                PurchaseOrderLineId = lines.First(l => l.ProductId == products[x.Sku].Id).Id,
                ProductId = products[x.Sku].Id,
                Quantity = x.Qty
            }).ToList();

            db.GoodsReceipts.Add(new GoodsReceipt
            {
                Code = code,
                PurchaseOrderId = order.Id,
                WarehouseId = warehouse.Id,
                ReceiptDate = date,
                Status = status,
                PostedAt = status == GoodsReceiptStatus.Posted ? DateTime.UtcNow : null,
                SupplierRef = $"DELTA-{code}",
                Notes = status == GoodsReceiptStatus.Posted ? "Phiếu nhập mẫu đã ghi sổ." : "Phiếu nhập mẫu đang nháp.",
                Lines = receiptLines
            });

            if (status != GoodsReceiptStatus.Posted)
                return;

            foreach (var line in receiptLines)
            {
                var balance = await db.StockBalances.FirstOrDefaultAsync(x => x.ProductId == line.ProductId && x.WarehouseId == warehouse.Id);
                if (balance is null)
                {
                    db.StockBalances.Add(new StockBalance
                    {
                        ProductId = line.ProductId,
                        WarehouseId = warehouse.Id,
                        Qty = line.Quantity,
                        MinQty = 2
                    });
                }
                else
                {
                    balance.Qty += line.Quantity;
                }

                db.InventoryTxns.Add(new InventoryTxn
                {
                    ProductId = line.ProductId,
                    WarehouseId = warehouse.Id,
                    Type = InventoryTxnType.In,
                    QtyChange = line.Quantity,
                    Reference = code,
                    Note = "Nhập kho mua hàng"
                });
            }
        }

        await Receive("PN-002", hcm, DateTime.Today.AddDays(-2), GoodsReceiptStatus.Posted,
            ("SP-013", 20), ("SP-019", 4), ("SP-027", 10));
        await Receive("PN-003", hn, DateTime.Today.AddDays(-1), GoodsReceiptStatus.Posted,
            ("SP-013", 10), ("SP-031", 30), ("SP-044", 20));
        await Receive("PN-004", hcm, DateTime.Today, GoodsReceiptStatus.Draft,
            ("SP-027", 5), ("SP-044", 10));
        await db.SaveChangesAsync();
    }

    private static async Task SeedOpeningStockAsync(AppDbContext db)
    {
        if (await db.InventoryTxns.AnyAsync() || !await db.StockBalances.AnyAsync())
            return;

        var balances = await db.StockBalances.Include(x => x.Warehouse).ToListAsync();
        foreach (var balance in balances)
        {
            db.InventoryTxns.Add(new InventoryTxn
            {
                ProductId = balance.ProductId,
                WarehouseId = balance.WarehouseId,
                Type = InventoryTxnType.Adjust,
                QtyChange = balance.Qty,
                Reference = "OPENING",
                Note = $"Tồn đầu kỳ {balance.Warehouse.Code}"
            });
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedCommercialAsync(AppDbContext db)
    {
        if (await db.SalesOrders.AnyAsync() || !await db.Partners.AnyAsync() || !await db.Products.AnyAsync())
            return;

        var customer = await db.Partners.FirstAsync(x => x.Code == "KH-001");
        var supplier = await db.Partners.FirstAsync(x => x.Code == "NCC-01");
        var hcm = await db.Warehouses.FirstAsync(x => x.Code == "WH-HCM");
        var hn = await db.Warehouses.FirstAsync(x => x.Code == "WH-HN");
        var laptop = await db.Products.FirstAsync(x => x.Sku == "SP-001");
        var mouse = await db.Products.FirstAsync(x => x.Sku == "SP-002");

        var salesLine = new SalesOrderLine
        {
            ProductId = laptop.Id,
            Qty = 2,
            UnitPrice = laptop.SalePrice,
            LineTotal = 2 * laptop.SalePrice
        };
        var salesOrder = new SalesOrder
        {
            Number = "SO-001",
            PartnerId = customer.Id,
            WarehouseId = hcm.Id,
            OrderDate = DateTime.Today.AddDays(-2),
            Status = SalesOrderStatus.Confirmed,
            ConfirmedAt = DateTime.UtcNow.AddDays(-2),
            TaxRate = 0.08m,
            Notes = "Đơn mẫu. Kho đề xuất HCM, xuất một phần từ HN.",
            Lines = [salesLine]
        };
        salesOrder.SubTotal = salesLine.LineTotal;
        salesOrder.TaxAmount = Math.Round(salesOrder.SubTotal * salesOrder.TaxRate, 2);
        salesOrder.Total = salesOrder.SubTotal + salesOrder.TaxAmount;
        db.SalesOrders.Add(salesOrder);
        await db.SaveChangesAsync();

        var issueLine = new GoodsIssueLine
        {
            SalesOrderLineId = salesLine.Id,
            ProductId = laptop.Id,
            Quantity = 1,
            Note = "Xuất từ kho Hà Nội vì HCM giữ hàng trưng bày"
        };
        var issue = new GoodsIssue
        {
            Code = "PX-001",
            SalesOrderId = salesOrder.Id,
            WarehouseId = hn.Id,
            IssueDate = DateTime.Today.AddDays(-1),
            Status = GoodsIssueStatus.Posted,
            PostedAt = DateTime.UtcNow.AddDays(-1),
            Notes = "Phiếu mẫu. Kho xuất khác kho đề xuất.",
            Lines = [issueLine]
        };
        db.GoodsIssues.Add(issue);

        var hnBalance = await db.StockBalances.FirstAsync(x => x.ProductId == laptop.Id && x.WarehouseId == hn.Id);
        hnBalance.Qty -= 1;
        db.InventoryTxns.Add(new InventoryTxn
        {
            ProductId = laptop.Id,
            WarehouseId = hn.Id,
            Type = InventoryTxnType.Out,
            QtyChange = -1,
            Reference = "PX-001",
            Note = "Xuất kho bán hàng"
        });

        var invoiceLine = new InvoiceLine
        {
            SalesOrderLineId = salesLine.Id,
            ProductId = laptop.Id,
            Qty = 1,
            UnitPrice = laptop.SalePrice,
            LineTotal = laptop.SalePrice
        };
        var invoice = new Invoice
        {
            Number = "HD-001",
            PartnerId = customer.Id,
            SalesOrderId = salesOrder.Id,
            IssueDate = DateTime.Today.AddDays(-1),
            DueDate = DateTime.Today.AddDays(29),
            Status = InvoiceStatus.PartiallyPaid,
            TaxRate = 0.08m,
            Notes = "Hóa đơn phần đã xuất.",
            Lines = [invoiceLine]
        };
        invoice.SubTotal = invoiceLine.LineTotal;
        invoice.TaxAmount = Math.Round(invoice.SubTotal * invoice.TaxRate, 2);
        invoice.Total = invoice.SubTotal + invoice.TaxAmount;
        invoice.PaidAmount = Math.Round(invoice.Total / 2, 0);
        invoice.IssuedAt = DateTime.UtcNow.AddDays(-1);
        db.Invoices.Add(invoice);

        var payment = new Payment
        {
            Number = "PT-001",
            PartnerId = customer.Id,
            PaidAt = DateTime.Today,
            Amount = invoice.PaidAmount,
            Method = PaymentMethod.BankTransfer,
            Status = CustomerPaymentStatus.Posted,
            PostedAt = DateTime.UtcNow,
            ReferenceNumber = "VCB-88991",
            Notes = "Thu một phần hóa đơn HD-001"
        };
        db.Payments.Add(payment);
        await db.SaveChangesAsync();
        db.PaymentAllocations.Add(new PaymentAllocation
        {
            PaymentId = payment.Id,
            InvoiceId = invoice.Id,
            Amount = invoice.PaidAmount
        });

        var purchaseLine = new PurchaseOrderLine
        {
            ProductId = mouse.Id,
            Qty = 30,
            UnitPrice = mouse.CostPrice,
            LineTotal = 30 * mouse.CostPrice
        };
        var purchase = new PurchaseOrder
        {
            Number = "PO-001",
            PartnerId = supplier.Id,
            WarehouseId = hcm.Id,
            OrderDate = DateTime.Today.AddDays(-5),
            Status = PurchaseOrderStatus.Confirmed,
            ConfirmedAt = DateTime.UtcNow.AddDays(-5),
            TaxRate = 0.08m,
            Notes = "Đơn mua mẫu. Kho đề xuất HCM.",
            Lines = [purchaseLine]
        };
        purchase.SubTotal = purchaseLine.LineTotal;
        purchase.TaxAmount = Math.Round(purchase.SubTotal * purchase.TaxRate, 2);
        purchase.Total = purchase.SubTotal + purchase.TaxAmount;
        db.PurchaseOrders.Add(purchase);
        await db.SaveChangesAsync();

        var receipt = new GoodsReceipt
        {
            Code = "PN-001",
            PurchaseOrderId = purchase.Id,
            WarehouseId = hcm.Id,
            ReceiptDate = DateTime.Today.AddDays(-3),
            Status = GoodsReceiptStatus.Posted,
            PostedAt = DateTime.UtcNow.AddDays(-3),
            SupplierRef = "DELTA-7788",
            Notes = "Nhập một phần đơn mua.",
            Lines =
            [
                new GoodsReceiptLine
                {
                    PurchaseOrderLineId = purchaseLine.Id,
                    ProductId = mouse.Id,
                    Quantity = 10
                }
            ]
        };
        db.GoodsReceipts.Add(receipt);
        var hcmMouse = await db.StockBalances.FirstAsync(x => x.ProductId == mouse.Id && x.WarehouseId == hcm.Id);
        hcmMouse.Qty += 10;
        db.InventoryTxns.Add(new InventoryTxn
        {
            ProductId = mouse.Id,
            WarehouseId = hcm.Id,
            Type = InventoryTxnType.In,
            QtyChange = 10,
            Reference = "PN-001",
            Note = "Nhập kho mua hàng"
        });

        var billLine = new VendorBillLine
        {
            PurchaseOrderLineId = purchaseLine.Id,
            ProductId = mouse.Id,
            Qty = 10,
            UnitPrice = mouse.CostPrice,
            LineTotal = 10 * mouse.CostPrice
        };
        var bill = new VendorBill
        {
            Number = "HDM-001",
            SupplierNumber = "DELTA-INV-100",
            PartnerId = supplier.Id,
            PurchaseOrderId = purchase.Id,
            BillDate = DateTime.Today.AddDays(-3),
            DueDate = DateTime.Today.AddDays(27),
            Status = VendorBillStatus.Posted,
            TaxRate = 0.08m,
            PostedAt = DateTime.UtcNow.AddDays(-3),
            Notes = "Hóa đơn phần đã nhập.",
            Lines = [billLine]
        };
        bill.SubTotal = billLine.LineTotal;
        bill.TaxAmount = Math.Round(bill.SubTotal * bill.TaxRate, 2);
        bill.Total = bill.SubTotal + bill.TaxAmount;
        db.VendorBills.Add(bill);

        await db.SaveChangesAsync();
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
