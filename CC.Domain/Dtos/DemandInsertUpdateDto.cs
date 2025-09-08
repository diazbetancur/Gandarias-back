namespace CC.Domain.Dtos;

public class DemandInsertUpdateDto
{
    public Guid workstationId { get; set; }
    public int day { get; set; }
    public int hora { get; set; }
    public int fraccion { get; set; }
    public double effortRequired { get; set; }
    public Guid templateId { get; set; }
    public Guid? WorkstationWorkAreaId { get; set; }
    public Guid? Id { get; set; }
}