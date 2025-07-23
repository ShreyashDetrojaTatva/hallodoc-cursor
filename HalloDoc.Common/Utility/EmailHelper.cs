using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HalloDoc.Common.Constants;

namespace HalloDoc.Common.Utility
{
    public static class EmailHelper
    {
        public static async Task SendMail(string to, string subject, string body)
        {
            var smtpHost = ConfigItems.SmtpHost;
            var smtpPort = ConfigItems.SmtpPort;
            var smtpUser = ConfigItems.SmtpUser;
            var smtpPass = ConfigItems.SmtpPass;
            var from = ConfigItems.SmtpFrom;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage(from, to, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }
    }
} 