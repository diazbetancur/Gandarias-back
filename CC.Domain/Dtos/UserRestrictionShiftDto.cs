namespace CC.Domain.Dtos;

public class UserRestrictionShiftDto
{
    public Guid? Id { get; set; }
    public string Observation { get; set; }
    public bool IsRestricted { get; set; }

    public Guid UserId { get; set; }
    public string? UserFullName { get; set; }

    public Guid? ShiftTypeId { get; set; }
    public string ShiftTypeName { get; set; }
}