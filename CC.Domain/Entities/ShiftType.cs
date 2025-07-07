namespace CC.Domain.Entities;

public class ShiftType : EntityBase<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ShiftStructureType Structure { get; set; }
    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1lastStart { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2lastStart { get; set; }
    public bool finishNextDay { get; set; }
    public bool IsActive { get; set; }
}