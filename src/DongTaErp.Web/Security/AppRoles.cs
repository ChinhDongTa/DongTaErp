namespace DongTaErp.Web.Security;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Sales = "Sales";
    public const string Purchasing = "Purchasing";
    public const string Accounting = "Accounting";
    public const string Director = "Director";

    public const string All = "Admin,Sales,Purchasing,Accounting,Director";
    public const string Catalog = "Admin,Sales,Purchasing,Director";
    public const string SalesArea = "Admin,Sales,Accounting,Director";
    public const string PurchaseArea = "Admin,Purchasing,Accounting,Director";
    public const string Money = "Admin,Accounting,Director";
    public const string Hr = "Admin,Director";
    public const string Manage = "Admin";

    public static readonly string[] AllRoles = [Admin, Sales, Purchasing, Accounting, Director];

    public static string Display(string role) => role switch
    {
        Admin => "Quản trị hệ thống",
        Sales => "Bán hàng",
        Purchasing => "Mua hàng",
        Accounting => "Kế toán",
        Director => "Ban Giám đốc",
        _ => role
    };
}
