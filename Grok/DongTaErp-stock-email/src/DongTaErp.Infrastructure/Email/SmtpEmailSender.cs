using System.Net;
using System.Net.Mail;
using DongTaErp.Application.Common.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DongTaErp.Infrastructure.Email;

public class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IAppEmailSender
{
    public async Task SendAsync(IEnumerable<string> to, string subject, string body, CancellationToken ct = default)
    {
        var opt = options.Value;
        var recipients = to.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (recipients.Count == 0)
            return;
        if (!opt.Enabled)
        {
            logger.LogInformation("Email disabled. Would send '{Subject}' to {Recipients}", subject, string.Join(", ", recipients));
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(opt.From, opt.FromName),
            Subject = subject,
            Body = body
        };
        foreach (var recipient in recipients)
            message.To.Add(recipient);

        using var client = new SmtpClient(opt.Host, opt.Port)
        {
            EnableSsl = opt.UseSsl,
            Credentials = string.IsNullOrWhiteSpace(opt.UserName) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(opt.UserName, opt.Password)
        };
        await client.SendMailAsync(message, ct);
    }
}
