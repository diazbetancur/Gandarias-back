namespace CC.Domain.Dtos;

public class WorkstationDemandDto
{
    public Guid? Id { get; set; }
    public Guid WorkstationId { get; set; }
    public Guid TemplateId { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public double EffortRequired { get; set; }
}