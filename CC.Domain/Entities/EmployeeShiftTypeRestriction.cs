
namespace CC.Domain.Entities;

public class EmployeeShiftTypeRestriction : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public Guid ShiftTypeId { get; set; }
    public virtual ShiftType ShiftType { get; set; }
}