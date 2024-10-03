using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;


namespace TechWizWebApp.Services
{
    public interface IMailService
    {
        public Task SendMailAsync(string email, string subject, string message);
    }
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMailAsync(string emailReceiver, string subject, string message)
        {
            string emailSender = _configuration["MailSettings:Email"];
            string password = _configuration["MailSettings:Password"];
            string host = _configuration["MailSettings:Host"];
            int port = int.Parse(_configuration["MailSettings:Port"]);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse($"{emailSender}"));
            email.To.Add(MailboxAddress.Parse($"{emailReceiver}"));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            using var smtp = new SmtpClient();
            smtp.Connect(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSender, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);            
        }
    }
}
