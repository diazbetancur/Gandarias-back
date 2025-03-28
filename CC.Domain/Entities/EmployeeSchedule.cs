
namespace CC.Domain.Entities;

public class EmployeeSchedule : EntityBase<Guid>
{
    public string UserId { get; set; }
    public virtual User User { get; set; }
    public string AvailabilityType { get; set; } // Ejemplo: "Flexible", "Fixed", "Split", etc.
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; } 
    //public List<AvailablePeriod> AvailablePeriods { get; set; } // Para horarios flexibles
    //public List<UnavailablePeriod> UnavailablePeriods { get; set; } // Para horarios flexibles
    public bool IsActive { get; set; } // Si está activo
    public string Notes { get; set; } // Notas adicionales
}
