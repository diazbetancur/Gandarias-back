namespace CC.Domain.Entities;

public class WorkSchedule : EntityBase<Guid>
{
    public Guid WorkstationId { get; set; }  // Relación con el puesto de trabajo
    public Guid UserId { get; set; }  // Relación con el empleado
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ShiftType { get; set; } // Tipo de turno (partido, fijo, etc.)
}