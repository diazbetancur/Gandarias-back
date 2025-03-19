namespace CC.Domain.Dtos;

public class WorkstationDto : BaseDto<Guid>
{
    public string Name { get; set; }
    public bool? IsActive { get; set; }
}