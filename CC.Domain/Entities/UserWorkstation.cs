namespace CC.Domain.Entities;

public class UserWorkstation : EntityBase<Guid>
{
    public string UserId { get; set; }
    public virtual User User { get; set; }
    public Guid WorkstationId { get; set; }
    public virtual Workstation Workstation { get; set; }
}