using CC.Domain.Dtos;
using CC.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CC.Domain.Interfaces.Services;

public interface IRoleService
{
    Task<RoleDto> AddRoleAsync(RoleDto roleDto);

    Task<bool> DeleteRoleAsync(string roleId);

    Task<bool> EditRoleAsync(RoleDto roleDto);

    Task<List<RoleDto>> GetRolesAsync();

    Task<List<RoleDto>> GetRolesByIdAsync(string roleName);
}