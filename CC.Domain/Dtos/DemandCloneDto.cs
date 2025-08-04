namespace CC.Domain.Dtos;

public class DemandCloneDto
{
    public DayOfWeek day { get; set; }
    public Guid templateId { get; set; }
    public List<DayOfWeek> dayOfWeeks { get; set; }
}