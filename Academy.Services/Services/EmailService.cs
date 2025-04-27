using Academy.Core.Models.Email;
using Academy.Core.ServicesInterfaces;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp; 
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Services.Services
{
    public class EmailService : IEmailService
    {
        public readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }



        public async Task SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);
            Send(mailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("email", _emailSettings.FromEmail));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Content
            };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                // تعطيل التحقق من الشهادات (غير موصى به في بيئة الإنتاج)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

               

                // الاتصال بخادم SMTP باستخدام الخيارات المناسبة
                if (_emailSettings.Port == 587)
                {
                    client.Connect(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                }
                else if (_emailSettings.Port == 465)
                {
                    client.Connect(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);
                }
                else
                {
                    throw new ArgumentException($"The specified port {_emailSettings.Port} is not supported.");
                }
                client.Authenticate(_emailSettings.Username, _emailSettings.Password);
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while sending email: {ex.Message}");
                throw; 
            }
            finally
            {
                if (client.IsConnected)
                {
                    client.Disconnect(true);
                }

                client.Dispose();
            }
        }

    }
}
