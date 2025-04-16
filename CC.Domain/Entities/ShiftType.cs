namespace CC.Domain.Entities;

public class ShiftType : EntityBase<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsFlexibleExit { get; set; }
    public ShiftStructureType Structure { get; set; }
    public ShiftSubType? SubType { get; set; }
    public ICollection<ShiftSchedule> Schedules { get; set; } = new List<ShiftSchedule>();
}