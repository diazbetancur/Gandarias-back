using CC.Domain.Enums;

namespace CC.Domain.Dtos;

public class EmployeeScheduleExceptionDto
{
    public Guid? Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserFullName { get; set; }

    public DateOnly Date { get; set; }
    public string RestrictionType { get; set; }
    public bool IsAdditionalRestriction { get; set; }

    public TimeSpan? AvailableFrom { get; set; }
    public TimeSpan? AvailableUntil { get; set; }

    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1End { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2End { get; set; }
}