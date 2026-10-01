namespace DongTaErp.Domain.Enums;

public enum PartnerType
{
    Customer = 1,
    Supplier = 2,
    Both = 3
}

public enum SalesOrderStatus
{
    Draft = 0,
    Confirmed = 1,
    Cancelled = 2
}

public enum InventoryTxnType
{
    In = 1,
    Out = 2,
    Adjust = 3
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum EmployeeStatus
{
    Probation = 0,
    Working = 1,
    OnLeave = 2,
    Resigned = 3
}

public enum LeaveType
{
    Annual = 1,
    Sick = 2,
    Unpaid = 3,
    Other = 9
}

public enum LeaveStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}
