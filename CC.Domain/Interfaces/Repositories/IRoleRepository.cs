using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role> AddRoleAsync(Role role);

    Task<bool> DeleteRoleAsync(string roleId);

    Task<bool> EditRoleAsync(Role role);

    Task<List<Role>> GetRolesAsync();

    Task<bool> RoleExistsAsync(string roleName);

    Task<Role> GetRoleByIdAsync(string roleId);

    Task<List<Role>> GetRolesByIdAsync(string roleName);
}