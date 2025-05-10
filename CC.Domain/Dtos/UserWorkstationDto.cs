namespace CC.Domain.Dtos;

public class UserWorkstationDto
{
    public Guid? Id { get; set; }

    public Guid UserId { get; set; }
    public string? UserName { get; set; }

    public Guid WorkstationId { get; set; }
    public string? WorkstationName { get; set; }

    public int Coverage { get; set; }

    public bool IsDelete { get; set; } = false;
}