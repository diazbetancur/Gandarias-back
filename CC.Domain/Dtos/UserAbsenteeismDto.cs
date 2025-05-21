namespace CC.Domain.Dtos;

public class UserAbsenteeismDto
{
    public Guid? Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AbsenteeismTypeId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? Observation { get; set; }
    public string? AbsenteeismTypeName { get; set; }
    public string? UserFullName { get; set; }
}