using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Repositories;

public interface IRolePermissionRepository : IERepositoryBase<RolePermission>
{
    Task<List<string>> GetRolesPermissionsAsync(List<string> permissons);
}