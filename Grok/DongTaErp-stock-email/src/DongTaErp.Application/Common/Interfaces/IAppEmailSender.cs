namespace DongTaErp.Application.Common.Interfaces;

public interface IAppEmailSender
{
    Task SendAsync(IEnumerable<string> to, string subject, string body, CancellationToken ct = default);
}
