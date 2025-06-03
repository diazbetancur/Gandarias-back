namespace CC.Domain.Dtos;

public class LawRestrictionDto
{
    public Guid? Id { get; set; }
    public string Description { get; set; }
    public int CantHours { get; set; }
}