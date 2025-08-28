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
        await _context.Database.EnsureCreatedAsync();
        await checkHireType();
        await CheckRolesAsync();
        await ChechAdminAsync();
        await CheckModuleAsync();
        await CheckRolePermissionAdminAsync();
        await CheckRolePermissionEmployeeAsync();
        await ChechEmployeeAsync();
        await CheckLawRestrictions();
        await ChechCoordinatorAsync();

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

    private async Task ChechCoordinatorAsync()
    {
        if (!_context.Users.Any())
        {
            var user = new UserDto
            {
                DNI = "Fichaje",
                Email = "horarios@restaurantegandarias.com",
                FirstName = "Fichaje",
                LastName = "Fichaje",
                NickName = "Fichaje",
                HireDate = DateTime.UtcNow,
                HireTypeId = _context.HireTypes.FirstOrDefault(x => x.Name == "Tiempo Completo")?.Id,
                PhoneNumber = "0000000000",
            };

            ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "FichajeGandarias1.");

            if (resultUserCreated.WasSuccessful)
            {
                IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Coordinator.ToString());
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
                new () { Name = "Extra", Id = Guid.NewGuid() },
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
                new () { Id = Guid.NewGuid(), Description = "Horas maximas de trabajo por dia", CantHours = 12,  DateCreated = DateTime.UtcNow },
                new () { Id = Guid.NewGuid(), Description = "Horas minimas entre jornadas", CantHours = 6, DateCreated = DateTime.UtcNow },
                new () { Id = Guid.NewGuid(), Description = "Horas minimas entre bloques de descanso", CantHours = 4, DateCreated = DateTime.UtcNow },
            });
            await _context.SaveChangesAsync();
        }
    }

    private async Task fillDataUser()
    {
        var tiempoCompleto = _context.HireTypes.FirstOrDefault(x => x.Name.ToLower().Trim() == "tiempo completo");
        var tiempoParcial = _context.HireTypes.FirstOrDefault(x => x.Name.ToLower().Trim() == "tiempo parcial");
        var extra = _context.HireTypes.FirstOrDefault(x => x.Name.ToLower().Trim() == "extra");

        var users = new List<UserDto>
{
    new UserDto { DNI = "60215317Y", Email = "jonaay09@gmail.com", FirstName = "DIANA ANGELINA", LastName = "ACOSTA RAMOS", NickName = "DIANA", HireDate = new DateTime(2024, 5, 22), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "631.143.030", HiredHours = 38 },
    new UserDto { DNI = "73263572Q", Email = "alvaroahechu06@gmail.com", FirstName = "ALVARO", LastName = "AHECHU GONZALEZ", NickName = "ALVARO", HireDate = new DateTime(2025, 4, 12), HireTypeId = tiempoParcial?.Id, PhoneNumber = "655.465.758", HiredHours = 16 },
    new UserDto { DNI = "X7926752D", Email = "ALHAGISAINE192@GMAIL.COM", FirstName = "SAINE", LastName = "ALHAGI", NickName = "ALHAGI", HireDate = new DateTime(2024, 4, 18), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "642.483.859", HiredHours = 38 },
    new UserDto { DNI = "34092742A", Email = "Garabatox2@gmail.com", FirstName = "JESUS MARIA", LastName = "ALVAREZ HOLGADO", NickName = "JESUS", HireDate = new DateTime(2017, 4, 24), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "646.575.138", HiredHours = 40 },
    new UserDto { DNI = "60681877B", Email = "melaniajimenez529@gmail.com", FirstName = "MELANIA", LastName = "ANACONA JIMENEZ", NickName = "MELANIE", HireDate = new DateTime(2024, 4, 2), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "698.618.947", HiredHours = 38 },
    new UserDto { DNI = "Y9994014E", Email = "oavila17928@gmail.com", FirstName = "OLMAN MANUEL", LastName = "AVILA SOLORZANO", NickName = "OLMAN", HireDate = new DateTime(2024, 7, 17), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "611.227.506", HiredHours = 38 },
    new UserDto { DNI = "72537219A", Email = "IKERAVILES329@GMAIL.COM", FirstName = "IKER", LastName = "AVILES CARRILLO", NickName = "IKER", HireDate = new DateTime(2025, 6, 9), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "622.447.263", HiredHours = 38 },
    new UserDto { DNI = "72508669L", Email = "marceloazocartrincado@gmail.com", FirstName = "MARCELO EDUAR", LastName = "AZOCAR TRINCADO", NickName = "MARCELO", HireDate = new DateTime(2014, 6, 23), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "690.257.575", HiredHours = 38 },
    new UserDto { DNI = "72507412G", Email = "marilynanita2023@gmail.com", FirstName = "MARILYN", LastName = "BARBERIS LAZO", NickName = "MARLYN", HireDate = new DateTime(2024, 5, 4), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "623.165.877", HiredHours = 38 },
    new UserDto { DNI = "Y1870763A", Email = "sihambenmustapha@gmail.com", FirstName = "SIHAM", LastName = "BENMUSTAPHA", NickName = "SIHAM", HireDate = new DateTime(2025, 4, 22), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "631.556.044", HiredHours = 38 },
    new UserDto { DNI = "73233304Q", Email = "ROSAIMELDA6225@GMAIL.COM", FirstName = "ROSA NEFES", LastName = "BURBANO ZUÑIGA", NickName = "ROSA", HireDate = new DateTime(2022, 8, 1), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "624.501.148", HiredHours = 38 },
    new UserDto { DNI = "Z2589164J", Email = "KARLA-BEIAP@HOTMAIL.COM", FirstName = "KARLA TATIANA", LastName = "CAEDENAS CAGUA", NickName = "KARLA", HireDate = new DateTime(2025, 7, 21), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "664.186.344", HiredHours = 38 },
    new UserDto { DNI = "79124678D", Email = "JEANPIERRECHICO02@GMAIL.COM", FirstName = "JEAN PIERRE", LastName = "CHICO PALOMINO", NickName = "JEAN PIERRE", HireDate = new DateTime(2025, 4, 7), HireTypeId = tiempoParcial?.Id, PhoneNumber = "624.987.763", HiredHours = 25 },
    new UserDto { DNI = "72599137M", Email = "angeldza77@gmail.com", FirstName = "HEYDER ANGEL", LastName = "DAZA RENDON", NickName = "ANGEL", HireDate = new DateTime(2022, 8, 16), HireTypeId = extra?.Id, PhoneNumber = "678.104.735", HiredHours = 0 },
    new UserDto { DNI = "Y8417347M", Email = "GRIZZI737@GMAIL.COM", FirstName = "ARTEM", LastName = "DEHTIAR", NickName = "ARTEM", HireDate = new DateTime(2024, 10, 8), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "604.917.064", HiredHours = 38 },
    new UserDto { DNI = "Y5951930G", Email = "diassef29@gmail.com", FirstName = "FALILOU", LastName = "DIASSE", NickName = "FALI", HireDate = new DateTime(2024, 5, 1), HireTypeId = extra?.Id, PhoneNumber = "688.419.896", HiredHours = 0 },
    new UserDto { DNI = "Y7250627Y", Email = "doburuth051993@gmail.com", FirstName = "RUTH", LastName = "DOBU", NickName = "RUTH", HireDate = new DateTime(2025, 4, 14), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "624.297.731", HiredHours = 38 },
    new UserDto { DNI = "77444821A", Email = "AHMADCHTIBI9@GMAIL.COM", FirstName = "AHMED", LastName = "ECH CHATBI ECH CHATBI", NickName = "AHMED", HireDate = new DateTime(2023, 10, 9), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "632.947.114", HiredHours = 38 },
    new UserDto { DNI = "X9757437D", Email = "Txikitxu2010@gmail.com", FirstName = "ANA CAROLINA", LastName = "FLORES GARCIA", NickName = "ANA", HireDate = new DateTime(2024, 9, 24), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "685.435.647", HiredHours = 38 },
    new UserDto { DNI = "Y1340799M", Email = "fozzattieva@gmail.com", FirstName = "EVA ELIZABETH", LastName = "FOZZATTI MENDEZ", NickName = "EVA", HireDate = new DateTime(2023, 5, 18), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "642.332.390", HiredHours = 38 },
    new UserDto { DNI = "34086154Q", Email = "avefenix2013.jg@gmail.com", FirstName = "JOSE ANTONIO", LastName = "GONZALEZ MARTIN", NickName = "JOSE", HireDate = new DateTime(2012, 5, 1), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "635.707.512", HiredHours = 38 },
    new UserDto { DNI = "73036937T", Email = "LAYDAIDROVOPARRA@GMAIL.COM", FirstName = "LAYDA ISABEL", LastName = "IDROVO PARRA", NickName = "LAYDA", HireDate = new DateTime(2018, 10, 1), HireTypeId = tiempoParcial?.Id, PhoneNumber = "637.946.917", HiredHours = 30 },
    new UserDto { DNI = "Z2992683C", Email = "islaleonr@gmail.com", FirstName = "RICHARD", LastName = "ISLA LEON", NickName = "RICHARD", HireDate = new DateTime(2025, 7, 21), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "672.593.451", HiredHours = 38 },
    new UserDto { DNI = "Y9391008F", Email = "Subinakharel0@gmail.com", FirstName = "SUBINA", LastName = "KHAREL SUBINA", NickName = "SUBINA", HireDate = new DateTime(2025, 7, 12), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "631.338.813", HiredHours = 38 },
    new UserDto { DNI = "73039315D", Email = "idoiates2017@gmail.com", FirstName = "IDOIA", LastName = "LERTXUNDI ETXEBERRIA", NickName = "IDOIA", HireDate = new DateTime(2024, 9, 27), HireTypeId = tiempoParcial?.Id, PhoneNumber = "667.603.805", HiredHours = 20 },
    new UserDto { DNI = "35772978C", Email = "rapidillo71.fm@gmail.com", FirstName = "FELIX", LastName = "MACARENO LARRAÑAGA", NickName = "FELIX", HireDate = new DateTime(2010, 2, 16), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "620.445.965", HiredHours = 38 },
    new UserDto { DNI = "32982839B", Email = "JEFF_GOOD19@HOTMAIL.COM", FirstName = "JEFFERSON STALIN", LastName = "MACIAS BUENO", NickName = "JEFFERSON", HireDate = new DateTime(2022, 10, 11), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "648.236.962", HiredHours = 38 },
    new UserDto { DNI = "72518390B", Email = "dmeloramirez26@gmail.com", FirstName = "IKER DANIEL", LastName = "MELO RAMIREZ", NickName = "DANI", HireDate = new DateTime(2025, 6, 14), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "632.456.615", HiredHours = 38 },
    new UserDto { DNI = "18176675M", Email = "gaure2005@gmail.com", FirstName = "GAURE", LastName = "MENDOZA DIAZ", NickName = "GAURE", HireDate = new DateTime(2025, 6, 25), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "637.441.894", HiredHours = 38 },
    new UserDto { DNI = "72839224H", Email = "OSWALDOPLAZA06@ICLOUD.COM", FirstName = "OSWALDO", LastName = "MOLINA PLAZA", NickName = "OSWALDO", HireDate = new DateTime(2025, 5, 28), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "665.782.618", HiredHours = 38 },
    new UserDto { DNI = "Y9424369H", Email = "navasdaniel0508@gmail.com", FirstName = "JOSE DANIEL", LastName = "NAVAS LOPEZ", NickName = "DANI", HireDate = new DateTime(2024, 7, 8), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "631.537.038", HiredHours = 38 },
    new UserDto { DNI = "Y7671723H", Email = "JULIETHNORENA76@GMAIL.COM", FirstName = "JULIETH FERNA", LastName = "NOREÑA VALENCIA", NickName = "JULIETH", HireDate = new DateTime(2023, 7, 26), HireTypeId = tiempoParcial?.Id, PhoneNumber = "688.756.258", HiredHours = 25 },
    new UserDto { DNI = "Z1479394Q", Email = "cursocomerciopaty@gmail.com", FirstName = "OLGA PATRICIA", LastName = "NOREÑA VALENCIA", NickName = "PATRICIA", HireDate = new DateTime(2025, 3, 12), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "661.122.974", HiredHours = 38 },
    new UserDto { DNI = "73277763Q", Email = "yessymaria8@gmail.com", FirstName = "YESSY MARIA", LastName = "PEREZ MENDOZA", NickName = "YESSY", HireDate = new DateTime(2018, 6, 4), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "671.902.971", HiredHours = 38 },
    new UserDto { DNI = "54294238R", Email = "MELVIN.PENALO@GMAIL.COM", FirstName = "MELVIN ARIEL", LastName = "PEÑALO DE LA PAZ", NickName = "MELVIN", HireDate = new DateTime(2022, 10, 10), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "653.026.849", HiredHours = 38 },
    new UserDto { DNI = "72545036T", Email = "julenzambrano2006@gmail.com", FirstName = "JULEN UNAI", LastName = "PINGARRON ZAMBRANO", NickName = "JULEN", HireDate = new DateTime(2025, 6, 17), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "612.494.959", HiredHours = 38 },
    new UserDto { DNI = "08147556J", Email = "Maria-magdalenapolanco@hotmail.com", FirstName = "MARIA MAGDALENA", LastName = "POLANCO GEREZ", NickName = "GINA", HireDate = new DateTime(2024, 11, 6), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "676.269.618", HiredHours = 38 },
    new UserDto { DNI = "X8325680W", Email = "ranescusimon@gmail.com", FirstName = "IULIANA SIMONA", LastName = "RANESCU", NickName = "SIMON", HireDate = new DateTime(2016, 8, 4), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "652.186.261", HiredHours = 38 },
    new UserDto { DNI = "73268921Y", Email = "ronyrivas@yahoo.es", FirstName = "RONY ALEXIS", LastName = "RIVAS TURCIOS", NickName = "RONY", HireDate = new DateTime(2011, 5, 2), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "687.372.143", HiredHours = 38 },
    new UserDto { DNI = "Z1211113F", Email = "Marlonrodriguezcool@gmail.com", FirstName = "MARLON FABRICI", LastName = "RODRIGUEZ COOL", NickName = "MARLON", HireDate = new DateTime(2024, 9, 16), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "688.872.647", HiredHours = 38 },
    new UserDto { DNI = "60858126B", Email = "RKARINNOHEMY@GMAIL.COM", FirstName = "KARIN NOHEMY", LastName = "RODRIGUEZ RODAS", NickName = "KARIN", HireDate = new DateTime(2024, 6, 21), HireTypeId = tiempoParcial?.Id, PhoneNumber = "605.738.991", HiredHours = 20 },
    new UserDto { DNI = "13340538D", Email = "NAOMIITZELL19@GMAIL.COM", FirstName = "NAOMI ITZEL", LastName = "ROJO MORENO", NickName = "NAOMI", HireDate = new DateTime(2025, 7, 7), HireTypeId = extra?.Id, PhoneNumber = "600.624.457", HiredHours = 0 },
    new UserDto { DNI = "08146765G", Email = "rosarioalenny8@gmail.com", FirstName = "ALENNY CHANEL", LastName = "ROSARIO MEDINA", NickName = "ALENNY", HireDate = new DateTime(2023, 6, 27), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "688.746.423", HiredHours = 38 },
    new UserDto { DNI = "73265868N", Email = "POOLRUIZ97@GMAIL.COM", FirstName = "JEAN POOL", LastName = "RUIZ SANCHEZ", NickName = "POOL", HireDate = new DateTime(2024, 10, 16), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "658.521.712", HiredHours = 38 },
    new UserDto { DNI = "34097572A", Email = "sanpedrojavier18@gmail.com", FirstName = "JAVIER", LastName = "SAN PEDRO MUÑOZ", NickName = "JSP", HireDate = new DateTime(2014, 5, 5), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "678.611.431", HiredHours = 38 },
    new UserDto { DNI = "35777429D", Email = "ainhoasantes@gmail.com", FirstName = "AINHOA", LastName = "SANTESTEBAN MARTINEZ", NickName = "AINHOA", HireDate = new DateTime(2013, 3, 1), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "679.892.070", HiredHours = 38 },
    new UserDto { DNI = "72518809Q", Email = "2007ibai@gmail.com", FirstName = "IBAI", LastName = "SARASOLA IPARRAGUIRRE", NickName = "IBAI", HireDate = new DateTime(2025, 5, 1), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "672.401.870", HiredHours = 38 },
    new UserDto { DNI = "72445176Y", Email = "ENEKO.SARASOLA@YAHOO.ES", FirstName = "ENEKO", LastName = "SARASOLA USANDIZAGA", NickName = "ENEKO", HireDate = new DateTime(2022, 6, 24), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "623.936.303", HiredHours = 38 },
    new UserDto { DNI = "72547594M", Email = "naiara-08@hotmail.com", FirstName = "NAIARA", LastName = "SARASUA RODRIGUEZ", NickName = "NAIARA", HireDate = new DateTime(2019, 9, 23), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "688.630.731", HiredHours = 38 },
    new UserDto { DNI = "32985468H", Email = "SSOCIAL385@GMAIL.COM", FirstName = "CRISTIANO", LastName = "SIQUEIRA DOS SANTOS", NickName = "CRISTIANO", HireDate = new DateTime(2016, 6, 6), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "632.214.620", HiredHours = 38 },
    new UserDto { DNI = "Y3392707Z", Email = "mirnavus1985@gmail.com", FirstName = "MIRNA VERONI", LastName = "UMANZOR SANDOVAL", NickName = "MIRNA", HireDate = new DateTime(2025, 5, 21), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "630.113.832", HiredHours = 38 },
    new UserDto { DNI = "72483169A", Email = "bvaquerizo@gmail.com", FirstName = "BORJA", LastName = "VAQUERIZO AYASTUY", NickName = "BORJA", HireDate = new DateTime(2011, 12, 2), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "635.204.324", HiredHours = 38 },
    new UserDto { DNI = "Z1030186K", Email = "DANIELVAZ61@HOTMAIL.COM", FirstName = "DANIEL ESTEBAN", LastName = "VASQUEZ SERNA", NickName = "DANIEL", HireDate = new DateTime(2024, 5, 30), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "688.604.637", HiredHours = 38 },
    new UserDto { DNI = "36529160P", Email = "eduard@restaurantegandarias.com", FirstName = "EDUARD", LastName = "VILLAMAYOR NOGUE", NickName = "EDUARD", HireDate = new DateTime(2018, 3, 28), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "636.884.171", HiredHours = 38 },
    new UserDto { DNI = "Y9110242W", Email = "Waqashokai786@gmail.com", FirstName = "MUHAMMAD", LastName = "WAQAS", NickName = "WAQAS", HireDate = new DateTime(2025, 4, 16), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "667.216.330", HiredHours = 38 },
    new UserDto { DNI = "32983606L", Email = "SAMUELWR11@ICLOUD.COM", FirstName = "ARSENIO SAMUEL", LastName = "WILSON RUIZ", NickName = "SAMU", HireDate = new DateTime(2023, 5, 2), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "624.484.119", HiredHours = 48 },
    new UserDto { DNI = "15959520G", Email = "mjzalakain@hotmail.es", FirstName = "MARIA JOSE", LastName = "ZALACAIN ITURRIOZ", NickName = "MARIJO", HireDate = new DateTime(2012, 9, 17), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "609.847.038", HiredHours = 38 },
    new UserDto { DNI = "34371250G", Email = "nassikau@gmail.com", FirstName = "NASSIKA", LastName = "ZAOUI BABAS", NickName = "NASSI", HireDate = new DateTime(2024, 1, 8), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "692.630.756", HiredHours = 38 },
    new UserDto { DNI = "34364514F", Email = "cristianzapatamora@gmail.com", FirstName = "CRISTIAN", LastName = "ZAPATA MORA", NickName = "CRISTIAN", HireDate = new DateTime(2023, 7, 4), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "641.118.286", HiredHours = 38 },
    new UserDto { DNI = "72599137M", Email = "angeldza77@gmail.com", FirstName = "HEYDER ANGEL", LastName = "DAZA RENDON", NickName = "ANGEL", HireDate = new DateTime(2022, 8, 16), HireTypeId = extra?.Id, PhoneNumber = "678.104.735", HiredHours = 0 },
    new UserDto { DNI = "Y8417347M", Email = "GRIZZI737@GMAIL.COM", FirstName = "ARTEM", LastName = "DEHTIAR", NickName = "ARTEM", HireDate = new DateTime(2024, 10, 8), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "604.917.064", HiredHours = 38 },
    new UserDto { DNI = "Y5951930G", Email = "diassef29@gmail.com", FirstName = "FALILOU", LastName = "DIASSE", NickName = "FALI", HireDate = new DateTime(2024, 5, 1), HireTypeId = extra?.Id, PhoneNumber = "688.419.896", HiredHours = 0 },
    new UserDto { DNI = "Y7250627Y", Email = "doburuth051993@gmail.com", FirstName = "RUTH", LastName = "DOBU", NickName = "RUTH", HireDate = new DateTime(2025, 4, 14), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "624.297.731", HiredHours = 38 },
    new UserDto { DNI = "77444821A", Email = "AHMADCHTIBI9@GMAIL.COM", FirstName = "AHMED", LastName = "ECH CHATBI ECH CHATBI", NickName = "AHMED", HireDate = new DateTime(2023, 10, 9), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "632.947.114", HiredHours = 38 },
    new UserDto { DNI = "X9757437D", Email = "Txikitxu2010@gmail.com", FirstName = "ANA CAROLINA", LastName = "FLORES GARCIA", NickName = "ANA", HireDate = new DateTime(2024, 9, 24), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "685.435.647", HiredHours = 38 },
    new UserDto { DNI = "Y1340799M", Email = "fozzattieva@gmail.com", FirstName = "EVA ELIZABETH", LastName = "FOZZATTI MENDEZ", NickName = "EVA", HireDate = new DateTime(2023, 5, 18), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "642.332.390", HiredHours = 38 },
    new UserDto { DNI = "34086154Q", Email = "avefenix2013.jg@gmail.com", FirstName = "JOSE ANTONIO", LastName = "GONZALEZ MARTIN", NickName = "JOSE", HireDate = new DateTime(2012, 5, 1), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "635.707.512", HiredHours = 38 },
    new UserDto { DNI = "73036937T", Email = "LAYDAIDROVOPARRA@GMAIL.COM", FirstName = "LAYDA ISABEL", LastName = "IDROVO PARRA", NickName = "LAYDA", HireDate = new DateTime(2018, 10, 1), HireTypeId = tiempoParcial?.Id, PhoneNumber = "637.946.917", HiredHours = 30 },
    new UserDto { DNI = "Z2992683C", Email = "islaleonr@gmail.com", FirstName = "RICHARD", LastName = "ISLA LEON", NickName = "RICHARD", HireDate = new DateTime(2025, 7, 21), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "672.593.451", HiredHours = 38 },
    new UserDto { DNI = "Y9391008F", Email = "Subinakharel0@gmail.com", FirstName = "SUBINA", LastName = "KHAREL SUBINA", NickName = "SUBINA", HireDate = new DateTime(2025, 7, 12), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "631.338.813", HiredHours = 38 },
    new UserDto { DNI = "73039315D", Email = "idoiates2017@gmail.com", FirstName = "IDOIA", LastName = "LERTXUNDI ETXEBERRIA", NickName = "IDOIA", HireDate = new DateTime(2024, 9, 27), HireTypeId = tiempoParcial?.Id, PhoneNumber = "667.603.805", HiredHours = 20 },
    new UserDto { DNI = "35772978C", Email = "rapidillo71.fm@gmail.com", FirstName = "FELIX", LastName = "MACARENO LARRAÑAGA", NickName = "FELIX", HireDate = new DateTime(2010, 2, 16), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "620.445.965", HiredHours = 38 },
    new UserDto { DNI = "32982839B", Email = "JEFF_GOOD19@HOTMAIL.COM", FirstName = "JEFFERSON STALIN", LastName = "MACIAS BUENO", NickName = "JEFFERSON", HireDate = new DateTime(2022, 10, 11), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "648.236.962", HiredHours = 38 },
    new UserDto { DNI = "72518390B", Email = "dmeloramirez26@gmail.com", FirstName = "IKER DANIEL", LastName = "MELO RAMIREZ", NickName = "DANI", HireDate = new DateTime(2025, 6, 14), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "632.456.615", HiredHours = 38 },
    new UserDto { DNI = "18176675M", Email = "gaure2005@gmail.com", FirstName = "GAURE", LastName = "MENDOZA DIAZ", NickName = "GAURE", HireDate = new DateTime(2025, 6, 25), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "637.441.894", HiredHours = 38 },
    new UserDto { DNI = "72839224H", Email = "OSWALDOPLAZA06@ICLOUD.COM", FirstName = "OSWALDO", LastName = "MOLINA PLAZA", NickName = "OSWALDO", HireDate = new DateTime(2025, 5, 28), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "665.782.618", HiredHours = 38 },
    new UserDto { DNI = "Y9424369H", Email = "navasdaniel0508@gmail.com", FirstName = "JOSE DANIEL", LastName = "NAVAS LOPEZ", NickName = "DANI", HireDate = new DateTime(2024, 7, 8), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "631.537.038", HiredHours = 38 },
    new UserDto { DNI = "Y7671723H", Email = "JULIETHNORENA76@GMAIL.COM", FirstName = "JULIETH FERNA", LastName = "NOREÑA VALENCIA", NickName = "JULIETH", HireDate = new DateTime(2023, 7, 26), HireTypeId = tiempoParcial?.Id, PhoneNumber = "688.756.258", HiredHours = 25 },
    new UserDto { DNI = "Z1479394Q", Email = "cursocomerciopaty@gmail.com", FirstName = "OLGA PATRICIA", LastName = "NOREÑA VALENCIA", NickName = "PATRICIA", HireDate = new DateTime(2025, 3, 12), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "661.122.974", HiredHours = 38 },
    new UserDto { DNI = "73277763Q", Email = "yessymaria8@gmail.com", FirstName = "YESSY MARIA", LastName = "PEREZ MENDOZA", NickName = "YESSY", HireDate = new DateTime(2018, 6, 4), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "671.902.971", HiredHours = 38 },
    new UserDto { DNI = "54294238R", Email = "MELVIN.PENALO@GMAIL.COM", FirstName = "MELVIN ARIEL", LastName = "PEÑALO DE LA PAZ", NickName = "MELVIN", HireDate = new DateTime(2022, 10, 10), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "653.026.849", HiredHours = 38 },
    new UserDto { DNI = "72545036T", Email = "julenzambrano2006@gmail.com", FirstName = "JULEN UNAI", LastName = "PINGARRON ZAMBRANO", NickName = "JULEN", HireDate = new DateTime(2025, 6, 17), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "612.494.959", HiredHours = 38 },
    new UserDto { DNI = "08147556J", Email = "Maria-magdalenapolanco@hotmail.com", FirstName = "MARIA MAGDALENA", LastName = "POLANCO GEREZ", NickName = "GINA", HireDate = new DateTime(2024, 11, 6), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "676.269.618", HiredHours = 38 },
    new UserDto { DNI = "X8325680W", Email = "ranescusimon@gmail.com", FirstName = "IULIANA SIMONA", LastName = "RANESCU", NickName = "SIMON", HireDate = new DateTime(2016, 8, 4), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "652.186.261", HiredHours = 38 },
    new UserDto { DNI = "73268921Y", Email = "ronyrivas@yahoo.es", FirstName = "RONY ALEXIS", LastName = "RIVAS TURCIOS", NickName = "RONY", HireDate = new DateTime(2011, 5, 2), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "687.372.143", HiredHours = 38 },
    new UserDto { DNI = "Z1211113F", Email = "Marlonrodriguezcool@gmail.com", FirstName = "MARLON FABRICI", LastName = "RODRIGUEZ COOL", NickName = "MARLON", HireDate = new DateTime(2024, 9, 16), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "688.872.647", HiredHours = 38 },
    new UserDto { DNI = "60858126B", Email = "RKARINNOHEMY@GMAIL.COM", FirstName = "KARIN NOHEMY", LastName = "RODRIGUEZ RODAS", NickName = "KARIN", HireDate = new DateTime(2024, 6, 21), HireTypeId = tiempoParcial?.Id, PhoneNumber = "605.738.991", HiredHours = 20 },
    new UserDto { DNI = "13340538D", Email = "NAOMIITZELL19@GMAIL.COM", FirstName = "NAOMI ITZEL", LastName = "ROJO MORENO", NickName = "NAOMI", HireDate = new DateTime(2025, 7, 7), HireTypeId = extra?.Id, PhoneNumber = "600.624.457", HiredHours = 0 },
    new UserDto { DNI = "08146765G", Email = "rosarioalenny8@gmail.com", FirstName = "ALENNY CHANEL", LastName = "ROSARIO MEDINA", NickName = "ALENNY", HireDate = new DateTime(2023, 6, 27), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "688.746.423", HiredHours = 38 },
    new UserDto { DNI = "73265868N", Email = "POOLRUIZ97@GMAIL.COM", FirstName = "JEAN POOL", LastName = "RUIZ SANCHEZ", NickName = "POOL", HireDate = new DateTime(2024, 10, 16), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "658.521.712", HiredHours = 38 },
    new UserDto { DNI = "34097572A", Email = "sanpedrojavier18@gmail.com", FirstName = "JAVIER", LastName = "SAN PEDRO MUÑOZ", NickName = "JSP", HireDate = new DateTime(2014, 5, 5), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "678.611.431", HiredHours = 38 },
    new UserDto { DNI = "35777429D", Email = "ainhoasantes@gmail.com", FirstName = "AINHOA", LastName = "SANTESTEBAN MARTINEZ", NickName = "AINHOA", HireDate = new DateTime(2013, 3, 1), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "679.892.070", HiredHours = 38 },
    new UserDto { DNI = "72518809Q", Email = "2007ibai@gmail.com", FirstName = "IBAI", LastName = "SARASOLA IPARRAGUIRRE", NickName = "IBAI", HireDate = new DateTime(2025, 5, 1), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "672.401.870", HiredHours = 38 },
    new UserDto { DNI = "72445176Y", Email = "ENEKO.SARASOLA@YAHOO.ES", FirstName = "ENEKO", LastName = "SARASOLA USANDIZAGA", NickName = "ENEKO", HireDate = new DateTime(2022, 6, 24), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "623.936.303", HiredHours = 38 },
    new UserDto { DNI = "72547594M", Email = "naiara-08@hotmail.com", FirstName = "NAIARA", LastName = "SARASUA RODRIGUEZ", NickName = "NAIARA", HireDate = new DateTime(2019, 9, 23), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "688.630.731", HiredHours = 38 },
    new UserDto { DNI = "32985468H", Email = "SSOCIAL385@GMAIL.COM", FirstName = "CRISTIANO", LastName = "SIQUEIRA DOS SANTOS", NickName = "CRISTIANO", HireDate = new DateTime(2016, 6, 6), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "632.214.620", HiredHours = 38 },
    new UserDto { DNI = "Y3392707Z", Email = "mirnavus1985@gmail.com", FirstName = "MIRNA VERONI", LastName = "UMANZOR SANDOVAL", NickName = "MIRNA", HireDate = new DateTime(2025, 5, 21), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "630.113.832", HiredHours = 38 },
    new UserDto { DNI = "72483169A", Email = "bvaquerizo@gmail.com", FirstName = "BORJA", LastName = "VAQUERIZO AYASTUY", NickName = "BORJA", HireDate = new DateTime(2011, 12, 2), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "635.204.324", HiredHours = 38 },
    new UserDto { DNI = "Z1030186K", Email = "DANIELVAZ61@HOTMAIL.COM", FirstName = "DANIEL ESTEBAN", LastName = "VASQUEZ SERNA", NickName = "DANIEL", HireDate = new DateTime(2024, 5, 30), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "688.604.637", HiredHours = 38 },
    new UserDto { DNI = "36529160P", Email = "eduard@restaurantegandarias.com", FirstName = "EDUARD", LastName = "VILLAMAYOR NOGUE", NickName = "EDUARD", HireDate = new DateTime(2018, 3, 28), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "636.884.171", HiredHours = 38 },
    new UserDto { DNI = "Y9110242W", Email = "Waqashokai786@gmail.com", FirstName = "MUHAMMAD", LastName = "WAQAS", NickName = "WAQAS", HireDate = new DateTime(2025, 4, 16), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "667.216.330", HiredHours = 38 },
    new UserDto { DNI = "32983606L", Email = "SAMUELWR11@ICLOUD.COM", FirstName = "ARSENIO SAMUEL", LastName = "WILSON RUIZ", NickName = "SAMU", HireDate = new DateTime(2023, 5, 2), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "624.484.119", HiredHours = 48 },
    new UserDto { DNI = "15959520G", Email = "mjzalakain@hotmail.es", FirstName = "MARIA JOSE", LastName = "ZALACAIN ITURRIOZ", NickName = "MARIJO", HireDate = new DateTime(2012, 9, 17), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "609.847.038", HiredHours = 38 },
    new UserDto { DNI = "34371250G", Email = "nassikau@gmail.com", FirstName = "NASSIKA", LastName = "ZAOUI BABAS", NickName = "NASSI", HireDate = new DateTime(2024, 1, 8), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "692.630.756", HiredHours = 38 },
    new UserDto { DNI = "34364514F", Email = "cristianzapatamora@gmail.com", FirstName = "CRISTIAN", LastName = "ZAPATA MORA", NickName = "CRISTIAN", HireDate = new DateTime(2023, 7, 4), HireTypeId = tiempoCompleto?.Id, PhoneNumber = "641.118.286", HiredHours = 38 }
};

        var usersExist = await _userService.GetAllUsers().ConfigureAwait(false);

        foreach (var user in users)
        {
            var update = usersExist.FirstOrDefault(u => u.NickName.ToLower().Trim().Equals(user.NickName.ToLower().Trim()));

            if (update == null)
            {
                ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "Gandarias1.");
                if (resultUserCreated.WasSuccessful)
                {
                    IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Employee.ToString());
                }
            }
            else
            {
                //update.DNI = user.DNI;
                //update.Email = user.Email;
                //update.FirstName = user.FirstName;
                //update.LastName = user.LastName;
                //update.HireDate = user.HireDate;
                //update.HireTypeId = user.HireTypeId;
                //update.PhoneNumber = user.PhoneNumber;
                //update.HiredHours = user.HiredHours;
                //await _userService.UpdateUserAsync(update).ConfigureAwait(false);
            }
        }
        //foreach (var user in users)
        //{
        //    ActionResponse<User> resultUserCreated = await _userService.AddUserAsync(user, "Gandarias1.");
        //    if (resultUserCreated.WasSuccessful)
        //    {
        //        IdentityResult roleResult = await _userService.AddUserToRoleAsync(resultUserCreated.Result, RoleType.Employee.ToString());
        //    }
        //}
    }
}