namespace CC.Domain.Dtos;

public class ShiftTypeDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Structure { get; set; }
    public string? SubType { get; set; }
    public bool IsFlexibleExit { get; set; }
    public TimeSpan? Block1Start { get; set; }
    public TimeSpan? Block1End { get; set; }
    public TimeSpan? Block2Start { get; set; }
    public TimeSpan? Block2End { get; set; }
}