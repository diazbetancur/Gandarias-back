namespace CC.Domain.Entities;

public class Workstation : EntityBase<Guid>
{
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid WorkAreaId { get; set; }
    public virtual WorkArea WorkArea { get; set; }
}