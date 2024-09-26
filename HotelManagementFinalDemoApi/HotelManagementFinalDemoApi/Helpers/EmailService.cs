
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net;
using System.Threading.Tasks;

namespace HotelManagementFinalDemoApi.Helpers
{
    public class EmailService
    {

        private readonly string _smtpServer = "smtp.@gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "hotelhotelmanagement@gmail.com";
        private readonly string _smtpPass = "xrhn limc htpf hhsl";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_smtpUser, _smtpPass);

            var mailMessage = new MimeMessage();
            
                mailMessage.From.Add(new MailboxAddress("HotelManagement", _smtpUser));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"{body}"
            };


            mailMessage.To.Add(new MailboxAddress("HotelManagement", toEmail));

            await smtpClient.SendAsync(mailMessage);
        }
    }
}
