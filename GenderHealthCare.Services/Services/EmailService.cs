using GenderHealthCare.Contract.Services.Interfaces;
using GenderHealthCare.Core.Config;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace GenderHealthCare.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();

                Console.WriteLine($"📡 Connecting to SMTP server {_emailSettings.SmtpServer}:{_emailSettings.Port}...");
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

                Console.WriteLine("🔐 Authenticating...");
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

                Console.WriteLine($"📧 Sending email to {toEmail}...");
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
                Console.WriteLine($"✅ Email sent to {toEmail} with subject '{subject}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send email to {toEmail}: {ex.Message}");
                throw; // Optional: rethrow or log deeper
            }
        }
    }
}
