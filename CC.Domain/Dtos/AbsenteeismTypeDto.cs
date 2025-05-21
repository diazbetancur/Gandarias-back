namespace CC.Domain.Dtos;

public class AbsenteeismTypeDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; } = false;
}