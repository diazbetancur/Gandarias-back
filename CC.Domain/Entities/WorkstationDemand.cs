namespace CC.Domain.Entities;

public class WorkstationDemand : EntityBase<Guid>
{
    public Guid TemplateId { get; set; }
    public virtual WorkstationDemandTemplate Template { get; set; }

    public Guid WorkstationId { get; set; }
    public Workstation Workstation { get; set; }

    public DayOfWeek Day { get; set; }

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public double EffortRequired { get; set; }
}