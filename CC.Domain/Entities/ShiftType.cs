namespace CC.Domain.Entities;

public class ShiftType : EntityBase<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsFlexibleExit { get; set; }
    public ShiftStructureType Structure { get; set; }
    public ShiftSubType? SubType { get; set; }
    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1End { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2End { get; set; }
    public bool finishNextDay { get; set; } = false;
}