using CC.Domain.Helpers;
using CC.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.SqlTypes;
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

    public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml, byte[]? attach, string? name, string? mediaType)
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

            mailMessage.Headers.Add("MIME-Version", "1.0");
            mailMessage.Headers.Add("Content-Type", "text/html; charset=utf-8");

            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

            mailMessage.To.Add(toEmail);

            if (attach != null && attach.Length > 0)
            {
                var qrStream = new MemoryStream(attach);
                qrStream.Position = 0;

                var attachmentName = name ?? "attachment.png";
                var attachmentMediaType = mediaType ?? "image/png";

                var inlineAttachment = new Attachment(qrStream, attachmentName, attachmentMediaType)
                {
                    ContentId = "qr-inline-cid"
                };

                inlineAttachment.ContentDisposition.Inline = true;
                inlineAttachment.ContentDisposition.DispositionType = "inline";

                mailMessage.Attachments.Add(inlineAttachment);
            }

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