namespace CC.Domain.Dtos;

public class WorkstationDemandTemplateDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public List<WorkstationDemandDto>? Demands { get; set; }
}