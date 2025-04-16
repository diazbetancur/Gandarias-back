namespace CC.Domain.Entities;

public class ShiftSchedule : EntityBase<Guid>
{
    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1End { get; set; }

    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2End { get; set; }

    public Guid ShiftTypeId { get; set; }
    public virtual ShiftType ShiftType { get; set; }
}