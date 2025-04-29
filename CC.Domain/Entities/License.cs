namespace CC.Domain.Entities;

public class License : EntityBase<Guid>
{
    public string UserId { get; set; }
    public virtual User User { get; set; }
    public string Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsHalfDay { get; set; }
    public string? HalfPeriod { get; set; }
    public int? DaysRequested { get; set; }
    public string? Observation { get; set; }
    public bool IsDeleted { get; set; } = false;
}