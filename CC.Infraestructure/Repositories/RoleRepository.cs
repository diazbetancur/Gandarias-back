using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleRepository(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Role> AddRoleAsync(Role role)
        {
            await _roleManager.CreateAsync(role);
            return role;
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            Role? roleExit = await _roleManager.FindByIdAsync(roleId);
            if (roleExit != null)
            {
                await _roleManager.DeleteAsync(roleExit);
                return true;
            }
            return false;
        }

        public async Task<bool> EditRoleAsync(Role role)
        {
            IdentityResult result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<List<Role>> GetRolesByIdAsync(string roleName)
        {
            // Buscar el rol en la base de datos según el ID proporcionado
            List<Role> allRoles = await _roleManager.Roles.ToListAsync();

            // Definir la lista de roles a retornar según el rol encontrado
            List<string> rolesToReturn = roleName.ToUpper() switch
            {
                "SUPERADMIN" => new List<string> { "ADMIN", "SUPERADMIN", "ADMINCONTRACTOR", "TECHNICALCONTRACTOR" },
                "ADMIN" => new List<string> { "ADMIN", "ADMINCONTRACTOR", "TECHNICALCONTRACTOR" },
                "ADMINCONTRACTOR" => new List<string> { "ADMINCONTRACTOR", "TECHNICALCONTRACTOR" },
                "TECHNICALCONTRACTOR" => new List<string> { "TECHNICALCONTRACTOR" },
                _ => new List<string>() // Retorna lista vacía si el rol no es válido
            };

            // Obtener todos los roles de la base de datos y filtrar los que coincidan
            return allRoles.Where(r => rolesToReturn.Contains(r.NormalizedName)).ToList();
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<Role> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }
    }
}