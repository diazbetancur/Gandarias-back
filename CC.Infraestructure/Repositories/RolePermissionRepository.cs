using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Repositories;

public class RolePermissionRepository : ERepositoryBase<RolePermission>, IRolePermissionRepository
{
    private readonly DBContext _dbContext;

    public RolePermissionRepository(IQueryableUnitOfWork unitOfWork, DBContext dbContext) : base(unitOfWork)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> GetRolesPermissionsAsync(List<string> permissons)
    {
        List<string> permisssionsList = await _dbContext.RolePermissions
            .Include(mr => mr.Permission)
            .Include(mr => mr.Role)
            .Where(mr => permissons.Contains(mr.Role.Name))
            .Select(mr => mr.Permission.Name)
            .Distinct()
            .ToListAsync();

        return permisssionsList;
    }
}