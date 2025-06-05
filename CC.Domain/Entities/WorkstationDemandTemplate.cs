namespace CC.Domain.Entities;

public class WorkstationDemandTemplate : EntityBase<Guid>
{
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public ICollection<WorkstationDemand> Demands { get; set; }
}