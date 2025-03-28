

namespace CC.Domain.Dtos;

public class UserActivityLogDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; }
    public DateTime DateCreated { get; set; }
    public string IpAddress { get; set; }
}
