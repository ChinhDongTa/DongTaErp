namespace DongTaErp.Domain.Enums;

public enum PartnerType
{
    [Display(Name = "Khách hàng")]
    Customer = 1,

    [Display(Name = "Nhà cung cấp")]
    Supplier = 2,

    [Display(Name = "Khách hàng và nhà cung cấp")]
    Both = 3
}
/// <summary>
/// Trạng thái thanh toán lương
/// </summary>
public enum PayrollStatus
{
    [Display(Name = "Đang chờ")]
    Pending = 1,

    [Display(Name = "Đã xử lý")]
    Processed = 2,

    [Display(Name = "Đã thanh toán")]
    Paid = 3,

    [Display(Name = "Thất bại")]
    Failed = 4,

    [Display(Name = "Tạm giữ")]
    OnHold = 5
}
public enum SalesOrderStatus
{
    [Display(Name = "Nháp")]
    Draft = 0,

    [Display(Name = "Đã xác nhận")]
    Confirmed = 1,

    [Display(Name = "Đã hủy")]
    Cancelled = 2
}

public enum InventoryTxnType
{
    [Display(Name = "Nhập kho")]
    In = 1,

    [Display(Name = "Xuất kho")]
    Out = 2,

    [Display(Name = "Điều chỉnh")]
    Adjust = 3,

    [Display(Name = "Chuyển kho")]
    Transfer = 4
}

public enum WarehouseType
{
    [Display(Name = "Kho chính")]
    Main = 1,

    [Display(Name = "Kho chi nhánh")]
    Branch = 2,

    [Display(Name = "Kho trung chuyển")]
    Transit = 3,

    [Display(Name = "Kho ảo")]
    Virtual = 4
}

public enum Gender
{
    [Display(Name = "Nam")]
    Male = 1,

    [Display(Name = "Nữ")]
    Female = 2,

    [Display(Name = "Khác")]
    Other = 3
}

public enum EmployeeStatus
{
    [Display(Name = "Thử việc")]
    Probation = 0,

    [Display(Name = "Đang làm việc")]
    Working = 1,

    [Display(Name = "Đang nghỉ phép")]
    OnLeave = 2,

    [Display(Name = "Đã nghỉ việc")]
    Resigned = 3
}

public enum LeaveType
{
    [Display(Name = "Nghỉ phép năm")]
    Annual = 1,

    [Display(Name = "Nghỉ ốm")]
    Sick = 2,

    [Display(Name = "Nghỉ không lương")]
    Unpaid = 3,

    [Display(Name = "Khác")]
    Other = 9
}

public enum LeaveStatus
{
    [Display(Name = "Chờ duyệt")]
    Pending = 0,

    [Display(Name = "Đã duyệt")]
    Approved = 1,

    [Display(Name = "Từ chối")]
    Rejected = 2,

    [Display(Name = "Đã hủy")]
    Cancelled = 3
}