namespace CC.Domain.Entities;

public class WorkArea : EntityBase<Guid>
{
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public ICollection<Workstation> workstations { get; set; }
}