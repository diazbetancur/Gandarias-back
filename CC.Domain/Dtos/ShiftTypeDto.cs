namespace CC.Domain.Dtos;

public class ShiftTypeDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ShiftStructureType Structure { get; set; }
    public ShiftSubType? SubType { get; set; }
    public bool IsFlexibleExit { get; set; }
    public List<ShiftScheduleDto> Schedules { get; set; }
}

public class ShiftScheduleDto
{
    public Guid? Id { get; set; }
    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1End { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2End { get; set; }
}