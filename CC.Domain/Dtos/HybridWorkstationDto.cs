namespace CC.Domain.Dtos;

public class HybridWorkstationDto
{
    public Guid? Id { get; set; }

    public Guid WorkstationAId { get; set; }
    public string WorkstationAName { get; set; }

    public Guid WorkstationBId { get; set; }
    public string WorkstationBName { get; set; }

    public string? Description { get; set; }
    public bool IsDelete { get; set; } = false;
}