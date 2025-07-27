using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using HalloDoc.Common.Constants;

namespace HalloDoc.Common.Helpers
{
    public static class EmailHelper
    {
        public static async Task SendMail(string to, string subject, string body)
        {
            using var client = new SmtpClient(ConfigItems.SmtpHost, ConfigItems.SmtpPort)
            {
                Credentials = new NetworkCredential(ConfigItems.SmtpUser, ConfigItems.SmtpPass),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(ConfigItems.SmtpUser, ConfigItems.SmtpFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
        }
    }
} 