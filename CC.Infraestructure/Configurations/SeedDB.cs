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
        await CheckLawRestrictions();

        // Solo para crear los usuarios
        //await fillDataUser();
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

    private async Task CheckLawRestrictions()
    {
        if (!_context.LawRestrictions.Any())
        {
            await _context.LawRestrictions.AddRangeAsync(new List<LawRestriction>
            {
                new () { Id = Guid.NewGuid(), Description = "Horas maxima de trabajo por dia", CantHours = 12,  DateCreated = DateTime.UtcNow },
                new () { Id = Guid.NewGuid(), Description = "Horas minima entre jornadas", CantHours = 6, DateCreated = DateTime.UtcNow },
            });
            await _context.SaveChangesAsync();
        }
    }

    private async Task fillDataUser()
    {
        var users = new List<UserDto>
{
    new UserDto { DNI = "1", Email = "AHMADCHTIBI9@GMAIL.COM", FirstName = "AHMED", LastName = "ECH CHATBI ECH CHATBI", NickName = "AHMED", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "2", Email = "rosarioalenny8@gmail.com", FirstName = "ALENNY CHANEL", LastName = "ROSARIO MEDINA", NickName = "ALENNY", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "3", Email = "anesellovali1998@gmail.com", FirstName = "ANES", LastName = "ELLOVALI BENYAZID", NickName = "ANES", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "4", Email = "angeldaza77@gmail.com", FirstName = "ANGEL HEYDER", LastName = "DAZA RENDON", NickName = "ANGEL", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "5", Email = "GRIZZI737@GMAIL.COM", FirstName = "ARTEM", LastName = "DEHTIAR", NickName = "ARTEM", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "6", Email = "bvaquerizo@gmail.com", FirstName = "BORJA", LastName = "VAQUERIZO ALASTUY", NickName = "BORJA", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "7", Email = "Cjosegg2001@gmail.com", FirstName = "CHRISTIAN JOSE", LastName = "GARCIA GASCON", NickName = "CHRISTIAN", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "8", Email = "CRISTIANZAPATAMORA@GMAIL.COM", FirstName = "CRISTIAN", LastName = "ZAPATA MORA", NickName = "CRISTIAN", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "9", Email = "SSOCIAL385@GMAIL.COM", FirstName = "CRISTIANO", LastName = "SIQUEIRA DOS SANTOS", NickName = "CRISTIANO", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "10", Email = "navasdaniel0508@gmail.com", FirstName = "JOSE DANIEL", LastName = "NAVAS LOPEZ", NickName = "DANI", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "11", Email = "DANILOPERAZA9@GMAIL.COM", FirstName = "DANILO ENRIQUE", LastName = "PERAZA HENRIQUEZ", NickName = "DANILO", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "12", Email = "echegaray.yuri@gmail.com", FirstName = "YURI ALEJANDRO", LastName = "ECHEGARAY TORRES", NickName = "ECHEGARAY", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "13", Email = "emanuelvillacis1106@gmail.com", FirstName = "EMANUEL", LastName = "VILLACIS", NickName = "EMANUEL", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "14", Email = "FANYVERONICA2806@GMAIL.COM", FirstName = "FANY VERONICA", LastName = "MENDOZA GUZMAN", NickName = "FANY", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "15", Email = "gianellavalera@gmail.com", FirstName = "GIANELLA", LastName = "VALERA", NickName = "GIANELLA", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "16", Email = "harold.yataco@gmail.com", FirstName = "HAROLD", LastName = "YATACO", NickName = "HAROLD", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "17", Email = "HENRYARAUZ1286@GMAIL.COM", FirstName = "HENRY JAVIER", LastName = "ARAUZ HERRERA", NickName = "HENRY", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "18", Email = "JEISONANDRES012@GMAIL.COM", FirstName = "JEISON ANDRES", LastName = "ZAPATA CASTAÑO", NickName = "JEISON", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "19", Email = "johannama949@gmail.com", FirstName = "JOHANNA", LastName = "MALDONADO", NickName = "JOHANNA", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
    new UserDto { DNI = "20", Email = "JORGE.JAVIERALMENGOR@GMAIL.COM", FirstName = "JORGE JAVIER", LastName = "ALMENGOR QUINTERO", NickName = "JORGE", HireDate = DateTime.UtcNow, HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id, PhoneNumber = "123456789" },
};

        foreach (var user in users)
        {
            ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "Gandarias1.");
            if (resultUserCreated.WasSuccessful)
            {
                IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Employee.ToString());
            }
        }
    }
}