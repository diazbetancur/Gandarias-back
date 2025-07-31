namespace CC.Domain.Dtos;

public class UserShiftDto
{
    public Guid? Id { get; set; }

    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string Structure { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan Block1Start { get; set; }
    public TimeSpan? Block1lastStart { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2lastStart { get; set; }
}