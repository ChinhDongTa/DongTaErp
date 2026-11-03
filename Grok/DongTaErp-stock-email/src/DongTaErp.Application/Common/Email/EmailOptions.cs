namespace DongTaErp.Application.Common.Email;

public class EmailOptions
{
    public const string SectionName = "Email";
    public bool Enabled { get; set; }
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string From { get; set; } = "";
    public string FromName { get; set; } = "DongTa ERP";
    public string[] PurchasingRecipients { get; set; } = [];
    public int CheckHours { get; set; } = 6;
}
