namespace CC.Domain.Dtos;

public class QrToken
{
    public string EncryptedToken { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public DateOnly? WeekStart { get; set; }
    public DateOnly? WeekEnd { get; set; }
    public DateTime? ValidUntil { get; set; }
    public Guid? TokenId { get; set; }
}