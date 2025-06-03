namespace CC.Domain.Entities;

public class LawRestriction : EntityBase<Guid>
{
    public string Description { get; set; }
    public int CantHours { get; set; }
}