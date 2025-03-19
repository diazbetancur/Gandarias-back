namespace CC.Domain.Entities;

public class Permission : EntityBase<Guid>
{
    public string Name { get; set; }
}