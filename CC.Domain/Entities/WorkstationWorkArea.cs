namespace CC.Domain.Entities;

public class WorkstationWorkArea : EntityBase<Guid>
{
    public Guid WorkstationId { get; set; }
    public virtual Workstation Workstation { get; set; }
    public Guid WorkAreaId { get; set; }
    public virtual WorkArea WorkArea { get; set; }
}