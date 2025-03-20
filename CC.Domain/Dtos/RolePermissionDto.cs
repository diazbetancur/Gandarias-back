namespace CC.Domain.Dtos;

public class RolePermissionDto
{
    public Guid Id { get; set; }
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; }
    public string RoleName { get; set; }
    public Guid RoleId { get; set; }
}