namespace CC.Domain.Entities;

public class UserRestrictionShift : EntityBase<Guid>
{
    public string Observation { get; set; }
    public bool IsDelete { get; set; }
    public bool IsRestricted { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public Guid ShiftTypeId { get; set; }
    public virtual ShiftType ShiftType { get; set; }
}