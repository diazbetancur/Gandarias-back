using CC.Domain.Enums;

namespace CC.Domain.Entities;

public class EmployeeScheduleException : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public DateOnly Date { get; set; }
    public RestrictionType RestrictionType { get; set; }
    public bool IsAdditionalRestriction { get; set; }

    public TimeSpan? AvailableFrom { get; set; }
    public TimeSpan? AvailableUntil { get; set; }

    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1End { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2End { get; set; }
}