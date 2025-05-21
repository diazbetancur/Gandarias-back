namespace CC.Domain.Entities;

public class AbsenteeismType : EntityBase<Guid>
{
    public string Name { get; set; }
    public bool IsDeleted { get; set; } = false;
}