namespace CC.Domain.Dtos;

public class EmployeeShiftTypeRestrictionDto
{
    public Guid? Id { get; set; }
    public Guid UserId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public Guid ShiftTypeId { get; set; }
    public string? ShiftTypeName { get; set; }
    public string? ShiftTypeDescription { get; set; }
}