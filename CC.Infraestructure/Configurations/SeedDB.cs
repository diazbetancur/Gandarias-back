using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Enums;
using CC.Domain.Helpers;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
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
        await CheckRolesAsync();
        await ChechAdminAsync();
        //await CheckModuleAsync();
        //await CheckModuleRoleAsync();
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
        //if (!_context.Modules.Any())
        //{
        //    await _context.Modules.AddRangeAsync(new List<Permission>
        //    {
        //        new () { Name = "home", Id = Guid.NewGuid() },
        //        new () { Name = "Entity", Id = Guid.NewGuid() },
        //        new () { Name = "Category", Id = Guid.NewGuid() },
        //        new () { Name = "Fonts", Id = Guid.NewGuid() },
        //        new () { Name = "Criterial", Id = Guid.NewGuid() },
        //        new () { Name = "Fields", Id = Guid.NewGuid() },
        //    });
        //    await _context.SaveChangesAsync();
        //}
    }

    private async Task CheckModuleRoleAsync()
    {
        //if (!_context.ModuleRoles.Any())
        //{
        //    List<Permission> modules = await _context.Modules.ToListAsync();

        //    List<Role> roles = await _roleRepository.GetRolesAsync();

        //    Role? role = roles.FirstOrDefault(x => x.Name == RoleType.SuperAdmin.ToString());

        //    foreach (Permission module in modules)
        //    {
        //        await _context.ModuleRoles.AddAsync(new RolePermission
        //        {
        //            ModuleId = module.Id,
        //            Id = Guid.NewGuid(),
        //            RoleId = role.Id,
        //            DateCreated = DateTime.UtcNow
        //        });
        //    }
        //    await _context.SaveChangesAsync();
        //}
    }
}