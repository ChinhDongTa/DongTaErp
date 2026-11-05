namespace DongTaErp.ApiGateway.Security;

public static class ApiRoles
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
}
