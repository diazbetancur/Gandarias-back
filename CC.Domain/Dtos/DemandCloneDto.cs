namespace CC.Domain.Dtos;

public class DemandCloneDto
{
    public int day { get; set; }
    public Guid templateId { get; set; }
    public List<int> dayOfWeeks { get; set; }
}