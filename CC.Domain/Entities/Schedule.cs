namespace CC.Domain.Entities;

public class Schedule : EntityBase<Guid>
{
    public DateOnly Date { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public Guid WorkstationId { get; set; }
    public virtual Workstation Workstation { get; set; }

    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    public string? Observation { get; set; }
    public bool IsDeleted { get; set; } = false;

    public string? IdUpdate { get; set; }
    public DateTime? DateUpdate { get; set; }

    public bool Notified { get; set; } = false;
    public string? Token { get; set; } = string.Empty;
}