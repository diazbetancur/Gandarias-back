using System.ComponentModel.DataAnnotations.Schema;

namespace CC.Domain.Entities;

public class RolePermission : EntityBase<Guid>
{
    public Guid PermissionId { get; set; }
    public virtual Permission Module { get; set; }

    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }
}