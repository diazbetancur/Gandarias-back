namespace CC.Domain.Entities;

public class UserShift : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public DayOfWeek Day { get; set; }
    public ShiftStructureType Structure { get; set; }
    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1lastStart { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2lastStart { get; set; }
}