namespace CC.Domain.Entities;

public class HybridWorkstation : EntityBase<Guid>
{
    public Guid WorkstationAId { get; set; }
    public virtual Workstation WorkstationA { get; set; }

    public Guid WorkstationBId { get; set; }
    public virtual Workstation WorkstationB { get; set; }
    public Guid? WorkstationCId { get; set; }
    public virtual Workstation? WorkstationC { get; set; }

    public Guid? WorkstationDId { get; set; }
    public virtual Workstation? WorkstationD { get; set; }

    public string? Description { get; set; }
    public bool IsDeleted { get; set; } = false;
}