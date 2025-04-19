using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Enums;
using CC.Domain.Helpers;
using CC.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Configurations;

public class SeedDB
{
    private readonly DBContext _context;
    private readonly IUserRepository _userService;
    private readonly IRoleRepository _roleRepository;

    public SeedDB(DBContext context, IUserRepository userService, IRoleRepository userUnifOfWork)
    {
        _context = context;
        _userService = userService;
        _roleRepository = userUnifOfWork;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await checkHireType();
        await CheckRolesAsync();
        await ChechAdminAsync();
        await CheckModuleAsync();
        await CheckRolePermissionAdminAsync();
        await CheckRolePermissionEmployeeAsync();
        await ChechEmployeeAsync();
    }

    private async Task CheckRolesAsync()
    {
        if (!_context.Roles.Any())
        {
            foreach (string role in Enum.GetNames(typeof(RoleType)))
            {
                Role roleDto = new Role
                {
                    Name = role,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                    NormalizedName = role.ToUpper(),
                };
                await _roleRepository.AddRoleAsync(roleDto);
            }
        }
    }

    private async Task ChechAdminAsync()
    {
        if (!_context.Users.Any())
        {
            var user = new UserDto
            {
                DNI = "Admin",
                Email = "",
                FirstName = "admin",
                LastName = "admin",
                NickName = "Admin",
                HireDate = DateTime.UtcNow,
                HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id,
                PhoneNumber = "123456789",
            };

            ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "Gandarias1.");

            if (resultUserCreated.WasSuccessful)
            {
                IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Admin.ToString());
            }
        }
    }

    private async Task CheckModuleAsync()
    {
        if (!_context.Permissions.Any())
        {
            foreach (string permission in Enum.GetNames(typeof(Permissions)))
            {
                Permission p = new Permission
                {
                    Name = permission,
                    Id = Guid.NewGuid(),
                    DateCreated = DateTime.UtcNow
                };
                _context.Permissions.Add(p);
            }
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckRolePermissionAdminAsync()
    {
        if (!_context.RolePermissions.Any())
        {
            List<Permission> permissions = await _context.Permissions.ToListAsync();

            List<Role> roles = await _roleRepository.GetRolesAsync();

            Role? role = roles.FirstOrDefault(x => x.Name == RoleType.Admin.ToString());

            foreach (Permission permission in permissions)
            {
                await _context.RolePermissions.AddAsync(new RolePermission
                {
                    PermissionId = permission.Id,
                    Id = Guid.NewGuid(),
                    RoleId = role.Id,
                    DateCreated = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckRolePermissionEmployeeAsync()
    {
        var exist = await _context.RolePermissions
            .AnyAsync(x => x.Role.Name == RoleType.Employee.ToString() && x.Permission.Name == Permissions.WorkSchedule.ToString());

        if (!exist)
        {
            List<Permission> permissions = await _context.Permissions.ToListAsync();
            Permission permission = permissions.FirstOrDefault(x => x.Name == Permissions.WorkSchedule.ToString());
            List<Role> roles = await _roleRepository.GetRolesAsync();
            Role? role = roles.FirstOrDefault(x => x.Name == RoleType.Employee.ToString());
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                PermissionId = permission.Id,
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                DateCreated = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
    }

    private async Task ChechEmployeeAsync()
    {
        var exist = await _context.Users.AnyAsync(x => x.UserName == "Employee");
        if (!exist)
        {
            var user = new UserDto
            {
                DNI = "Employee",
                Email = "",
                FirstName = "Employee",
                LastName = "Employee",
                NickName = "Employee",
                HireDate = DateTime.UtcNow,
                HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id,
                PhoneNumber = "123456789",
            };

            ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "Gandarias1.");

            if (resultUserCreated.WasSuccessful)
            {
                IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Employee.ToString());
            }
        }
    }

    private async Task checkHireType()
    {
        if (!_context.HireTypes.Any())
        {
            await _context.HireTypes.AddRangeAsync(new List<HireType>
            {
                new () { Name = "Tiempo Completo", Id = Guid.NewGuid() },
                new () { Name = "Tiempo Parcial", Id = Guid.NewGuid() },
                new () { Name = "Temporal", Id = Guid.NewGuid() },
            });
            await _context.SaveChangesAsync();
        }
    }
}