using CC.Domain.Helpers;
using CC.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace CC.Infrastructure.EmailServices;

public class EmailService : IEmailService
{
    private readonly EmailServiceOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailServiceOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
    {
        try
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.SmtpUser),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_options.SmtpServer)
            {
                Port = _options.SmtpPort,
                EnableSsl = _options.EnableSsl,
                Credentials = new System.Net.NetworkCredential(_options.SmtpUser, _options.SmtpPassword),
                Timeout = 30000
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, "Error SMTP enviando email a {ToEmail}: {StatusCode}",
    toEmail, smtpEx.StatusCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error general enviando email a {ToEmail}", toEmail);
            throw;
        }
    }
}