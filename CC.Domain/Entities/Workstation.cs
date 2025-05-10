namespace CC.Domain.Entities;

public class Workstation : EntityBase<Guid>
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Guid WorkAreaId { get; set; }
    public virtual WorkArea WorkArea { get; set; }
    public virtual ICollection<UserWorkstation> UserWorkstations { get; set; } = new List<UserWorkstation>();
}