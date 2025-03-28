namespace CC.Domain.Entities;

public class PersonalEvent : EntityBase<Guid>
{
    public string UserId { get; set; }
    public virtual User User { get; set; }
}
