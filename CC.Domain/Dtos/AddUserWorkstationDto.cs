namespace CC.Domain.Dtos;

public class AddUserWorkstationDto
{
    public Guid UserId { get; set; }
    public List<WorkStationUserAddDto> workStations { get; set; }
}

public class WorkStationUserAddDto
{
    public Guid? Id { get; set; }
    public int Coverage { get; set; }
    public int Preference { get; set; }
    public Guid WorkstationId { get; set; }
}