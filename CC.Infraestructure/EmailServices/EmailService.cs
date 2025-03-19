using CC.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CC.Infrastructure.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _fromEmail;
        private readonly string _password;
        private readonly bool _enableSsl;

        public EmailService(string smtpServer, int port, string fromEmail, string password, bool enableSsl = true)
        {
            _smtpServer = smtpServer;
            _port = port;
            _fromEmail = fromEmail;
            _password = password;
            _enableSsl = enableSsl;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string password)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail),
                    Subject = "",
                    Body = "",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = _smtpServer;
                    smtpClient.Port = _port;
                    smtpClient.EnableSsl = _enableSsl;
                    smtpClient.Credentials = new System.Net.NetworkCredential(_fromEmail, _password);
                    smtpClient.Timeout = 10000;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar el correo electrónico: {ex.Message}", ex);
            }
        }
    }
}