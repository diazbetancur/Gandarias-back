namespace CC.Domain.Entities;

public class WorkArea : EntityBase<Guid>
{
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
}