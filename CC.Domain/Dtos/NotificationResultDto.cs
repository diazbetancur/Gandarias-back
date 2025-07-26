namespace CC.Domain.Dtos;

public class NotificationResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalProcessed { get; set; }
    public int SuccessfulSent { get; set; }
    public List<string> Errors { get; set; } = new();
}