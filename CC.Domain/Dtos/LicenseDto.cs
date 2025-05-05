namespace CC.Domain.Dtos;

public class LicenseDto
{
    public Guid? Id { get; set; }
    public string UserName { get; set; }
    public string Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsHalfDay { get; set; }
    public string? HalfPeriod { get; set; }
    public int? DaysRequested { get; set; }
    public string? Observation { get; set; }
}