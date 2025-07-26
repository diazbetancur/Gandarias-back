namespace CC.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string bodyHtml, byte[]? attach, string? name, string? mediaType);
    }
}