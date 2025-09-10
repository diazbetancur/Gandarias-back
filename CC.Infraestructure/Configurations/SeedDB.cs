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
    private readonly IUserService _userService;
    private readonly IRoleRepository _roleRepository;

    public SeedDB(DBContext context, IUserService userService, IRoleRepository userUnifOfWork)
    {
        _context = context;
        _userService = userService;
        _roleRepository = userUnifOfWork;
    }

    public async Task SeedAsync()
    {
        try
        {
            Console.WriteLine("🔍 Checking database connection...");

            // Test database connection first
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                Console.WriteLine("❌ Cannot connect to database");
                throw new Exception("Database connection failed");
            }

            Console.WriteLine("✅ Database connection successful");
            Console.WriteLine("🏗️ Ensuring database is created...");

            // Ensure database exists
            await _context.Database.EnsureCreatedAsync();

            Console.WriteLine("✅ Database creation verified");
            Console.WriteLine("🌱 Starting seed operations...");

            await checkHireType();
            Console.WriteLine("✅ HireTypes seeded");

            await CheckRolesAsync();
            Console.WriteLine("✅ Roles seeded");

            await ChechAdminAsync();
            Console.WriteLine("✅ Admin user seeded");

            await CheckModuleAsync();
            Console.WriteLine("✅ Permissions seeded");

            await CheckRolePermissionAdminAsync();
            Console.WriteLine("✅ Admin role permissions seeded");

            await CheckRolePermissionEmployeeAsync();
            Console.WriteLine("✅ Employee role permissions seeded");

            await ChechEmployeeAsync();
            Console.WriteLine("✅ Employee user seeded");

            await CheckLawRestrictions();
            Console.WriteLine("✅ Law restrictions seeded");

            await ChechCoordinatorAsync();
            Console.WriteLine("✅ Coordinator user seeded");

            Console.WriteLine("🎉 All seed operations completed successfully");

            // Solo para crear los usuarios (comentado por defecto)
            //await fillDataUser();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Seed operation failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private async Task CheckRolesAsync()
    {
        try
        {
            if (!_context.Roles.Any())
            {
                Console.WriteLine("Creating roles...");
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
            else
            {
                Console.WriteLine("Roles already exist, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating roles: {ex.Message}");
            throw;
        }
    }

    private async Task ChechAdminAsync()
    {
        try
        {
            var adminExists = await _context.Users.AnyAsync(x => x.UserName == "Admin");
            if (!adminExists)
            {
                Console.WriteLine("Creating admin user...");
                var hireType = await _context.HireTypes.FirstOrDefaultAsync(x => x.Name == "Tiempo Completo");

                var user = new UserDto
                {
                    DNI = "Admin",
                    Email = "",
                    FirstName = "admin",
                    LastName = "admin",
                    NickName = "Admin",
                    HireDate = DateTime.UtcNow,
                    HireTypeId = hireType?.Id,
                    PhoneNumber = "123456789",
                };

                ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "Gandarias1.");

                if (resultUserCreated.WasSuccessful)
                {
                    IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Admin.ToString());
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"Warning: Could not assign admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating admin user: {ex.Message}");
            throw;
        }
    }

    private async Task ChechCoordinatorAsync()
    {
        try
        {
            var coordinatorExists = await _context.Users.AnyAsync(x => x.UserName == "Fichaje");
            if (!coordinatorExists)
            {
                Console.WriteLine("Creating coordinator user...");
                var hireType = await _context.HireTypes.FirstOrDefaultAsync(x => x.Name == "Tiempo Completo");

                var user = new UserDto
                {
                    DNI = "Fichaje",
                    Email = "horarios@restaurantegandarias.com",
                    FirstName = "Fichaje",
                    LastName = "Fichaje",
                    NickName = "Fichaje",
                    HireDate = DateTime.UtcNow,
                    HireTypeId = hireType?.Id,
                    PhoneNumber = "0000000000",
                };

                ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "FichajeGandarias1.");

                if (resultUserCreated.WasSuccessful)
                {
                    IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Coordinator.ToString());
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"Warning: Could not assign coordinator role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Coordinator user already exists, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating coordinator user: {ex.Message}");
            throw;
        }
    }

    private async Task CheckModuleAsync()
    {
        try
        {
            if (!_context.Permissions.Any())
            {
                Console.WriteLine("Creating permissions...");
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
            else
            {
                Console.WriteLine("Permissions already exist, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating permissions: {ex.Message}");
            throw;
        }
    }

    private async Task CheckRolePermissionAdminAsync()
    {
        try
        {
            var hasAdminPermissions = await _context.RolePermissions
                .AnyAsync(x => x.Role.Name == RoleType.Admin.ToString());

            if (!hasAdminPermissions)
            {
                Console.WriteLine("Creating admin role permissions...");
                List<Permission> permissions = await _context.Permissions.ToListAsync();
                List<Role> roles = await _roleRepository.GetRolesAsync();
                Role? role = roles.FirstOrDefault(x => x.Name == RoleType.Admin.ToString());

                if (role != null)
                {
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
            else
            {
                Console.WriteLine("Admin role permissions already exist, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating admin role permissions: {ex.Message}");
            throw;
        }
    }

    private async Task CheckRolePermissionEmployeeAsync()
    {
        try
        {
            var exist = await _context.RolePermissions
                .AnyAsync(x => x.Role.Name == RoleType.Employee.ToString() && x.Permission.Name == Permissions.WorkSchedule.ToString());

            if (!exist)
            {
                Console.WriteLine("Creating employee role permissions...");
                List<Permission> permissions = await _context.Permissions.ToListAsync();
                Permission permission = permissions.FirstOrDefault(x => x.Name == Permissions.WorkSchedule.ToString());
                List<Role> roles = await _roleRepository.GetRolesAsync();
                Role? role = roles.FirstOrDefault(x => x.Name == RoleType.Employee.ToString());

                if (permission != null && role != null)
                {
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
            else
            {
                Console.WriteLine("Employee role permissions already exist, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating employee role permissions: {ex.Message}");
            throw;
        }
    }

    private async Task ChechEmployeeAsync()
    {
        try
        {
            var exist = await _context.Users.AnyAsync(x => x.UserName == "Employee");
            if (!exist)
            {
                Console.WriteLine("Creating employee user...");
                var hireType = await _context.HireTypes.FirstOrDefaultAsync(x => x.Name == "Tiempo Completo");

                var user = new UserDto
                {
                    DNI = "Employee",
                    Email = "",
                    FirstName = "Employee",
                    LastName = "Employee",
                    NickName = "Employee",
                    HireDate = DateTime.UtcNow,
                    HireTypeId = hireType?.Id,
                    PhoneNumber = "123456789",
                };

                ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "Gandarias1.");

                if (resultUserCreated.WasSuccessful)
                {
                    IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Employee.ToString());
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"Warning: Could not assign employee role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Employee user already exists, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating employee user: {ex.Message}");
            throw;
        }
    }

    private async Task checkHireType()
    {
        try
        {
            if (!_context.HireTypes.Any())
            {
                Console.WriteLine("Creating hire types...");
                await _context.HireTypes.AddRangeAsync(new List<HireType>
                {
                    new () { Name = "Tiempo Completo", Id = Guid.NewGuid() },
                    new () { Name = "Tiempo Parcial", Id = Guid.NewGuid() },
                    new () { Name = "Temporal", Id = Guid.NewGuid() },
                    new () { Name = "Extra", Id = Guid.NewGuid() },
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Hire types already exist, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating hire types: {ex.Message}");
            throw;
        }
    }

    private async Task CheckLawRestrictions()
    {
        try
        {
            if (!_context.LawRestrictions.Any())
            {
                Console.WriteLine("Creating law restrictions...");
                await _context.LawRestrictions.AddRangeAsync(new List<LawRestriction>
                {
                    new () { Id = Guid.NewGuid(), Description = "Horas maximas de trabajo por dia", CantHours = 12,  DateCreated = DateTime.UtcNow },
                    new () { Id = Guid.NewGuid(), Description = "Horas minimas entre jornadas", CantHours = 6, DateCreated = DateTime.UtcNow },
                    new () { Id = Guid.NewGuid(), Description = "Horas minimas entre bloques de descanso", CantHours = 4, DateCreated = DateTime.UtcNow },
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Law restrictions already exist, skipping...");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating law restrictions: {ex.Message}");
            throw;
        }
    }
}