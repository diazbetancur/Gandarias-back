namespace CC.Domain.Entities;

public class HybridWorkstation : EntityBase<Guid>
{
    public Guid WorkstationAId { get; set; }
    public virtual Workstation WorkstationA { get; set; }

    public Guid WorkstationBId { get; set; }
    public virtual Workstation WorkstationB { get; set; }

    public string? Description { get; set; } // Opcional: para describir el par
    public bool IsDeleted { get; set; } = false;
}