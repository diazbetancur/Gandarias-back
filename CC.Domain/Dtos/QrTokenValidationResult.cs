namespace CC.Domain.Dtos;

public class QrTokenValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public DateOnly? WeekStart { get; set; }
    public DateOnly? WeekEnd { get; set; }
    public DateTime? ValidUntil { get; set; }
    public Guid? TokenId { get; set; }
}