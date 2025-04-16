namespace CC.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string password);
    }
}