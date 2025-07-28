namespace CC.Domain.Dtos;

public class ScheduleDto
{
    public Guid? Id { get; set; }

    public DateOnly Date
    {
        get; set;
    }

    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserNickName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserFullName { get; set; }

    public Guid WorkstationId { get; set; }
    public string? WorkstationName { get; set; }

    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    public string? Observation { get; set; }
    public bool IsDeleted { get; set; }

    public string? IdUpdate { get; set; }
    public DateTime? DateUpdate { get; set; }

    public Guid? WorkstationWorkAreaId { get; set; }
    public string? WorkstationWorkAreaName { get; set; }
}