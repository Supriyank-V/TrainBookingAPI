using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace TrainBookingAPI.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public void SendEmail(Email request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetValue<string>("Smtp:FromAddress")));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetValue<string>("Smtp:Host"), _config.GetValue<int>("Smtp:Port"), SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetValue<string>("Smtp:FromAddress"), _config.GetValue<string>("Smtp:Password"));
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(Email request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetValue<string>("Smtp:FromAddress")));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_config.GetValue<string>("Smtp:Host"), _config.GetValue<int>("Smtp:Port"), SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_config.GetValue<string>("Smtp:FromAddress"), _config.GetValue<string>("Smtp:Password"));
                    await smtp.SendAsync(email);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                    smtp.Dispose();
                }
            }
        }
    }
}
