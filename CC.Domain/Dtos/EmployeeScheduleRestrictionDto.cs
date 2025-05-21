using CC.Domain.Enums;

namespace CC.Domain.Dtos;

public class EmployeeScheduleRestrictionDto
{
    public Guid? Id { get; set; }
    public Guid UserId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public RestrictionType RestrictionType { get; set; }

    public TimeSpan? AvailableFrom { get; set; }
    public TimeSpan? AvailableUntil { get; set; }

    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1End { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2End { get; set; }
}