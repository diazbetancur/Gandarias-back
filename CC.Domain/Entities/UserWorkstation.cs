namespace CC.Domain.Entities;

public class UserWorkstation : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid WorkstationId { get; set; }
    public virtual Workstation Workstation { get; set; }
    public int Coverage { get; set; }
    public bool IsDelete { get; set; } = false;
    public int Preference { get; set; }
}