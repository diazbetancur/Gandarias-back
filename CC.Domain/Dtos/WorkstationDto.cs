namespace CC.Domain.Dtos;

public class WorkstationDto : BaseDto<Guid>
{
    public string Name { get; set; }
    public bool? IsActive { get; set; }
    public Guid WorkAreaId { get; set; }
    public string? WorkAreaName { get; set; }
    public bool? IsDeleted { get; set; } = false;
}