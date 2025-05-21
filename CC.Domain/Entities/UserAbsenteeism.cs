namespace CC.Domain.Entities;

public class UserAbsenteeism : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public Guid AbsenteeismTypeId { get; set; }
    public virtual AbsenteeismType AbsenteeismType { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Observation { get; set; }
}