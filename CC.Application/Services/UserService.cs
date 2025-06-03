using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Enums;
using CC.Domain.Helpers;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CC.Application.Services
{
    public class UserService : ServiceBase<User, UserDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IRolePermissionRepository rolePermissionRepository) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<ActionResponse<User>> AddUserAsync(UserDto user, string password)
        {
            password = Extensions.GenerateRandomPassword();
            var existingUser = await _userRepository.GetUserAsync(user.DNI);
            if (existingUser != null)
            {
                if (existingUser.IsDelete == true)
                {
                    existingUser.IsDelete = false;
                    existingUser.Email = user.Email;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.NickName = user.NickName;
                    existingUser.IsActive = true;
                    existingUser.LawApply = user.LawApply;
                    existingUser.HiredHours = user.HiredHours;
                    existingUser.ComplementHours = user.ComplementHours;
                    existingUser.ExtraHours = user.ExtraHours;
                    existingUser.CantPartTimeSchedule = user.CantPartTimeSchedule;

                    var updateResult = await _userRepository.UpdateUserAsync(existingUser);

                    if (updateResult.Succeeded)
                    {
                        return new ActionResponse<User>
                        {
                            WasSuccessful = true,
                            Message = "El usuario se creo correctamente."
                        };
                    }

                    return new ActionResponse<User>
                    {
                        WasSuccessful = true,
                        Message = $"No se logro crear el usuario, por favor intentelo nuevamente.",
                    };
                }

                return new ActionResponse<User>
                {
                    WasSuccessful = true,
                    Message = $"El usuario ya existe",
                };
            }
            user.HireDate = user.HireDate != null ? user.HireDate : DateTime.UtcNow;
            ActionResponse<User> resultUserCreated = await _userRepository.AddUserAsync(user, password);
            // Pendiente envio de correo electronico
            if (resultUserCreated.WasSuccessful)
            {
                IdentityResult roleResult = await AddUserToRoleAsync(resultUserCreated.Result, RoleType.Employee.ToString());
                if (roleResult.Succeeded)
                {
                    return resultUserCreated;
                }

                return new ActionResponse<User>
                {
                    WasSuccessful = true,
                    Result = resultUserCreated.Result,
                    Message = $"El usuario fue creado correctamente, pero no se pudo asignar el rol {user.RolName}.",
                };
            }
            return new ActionResponse<User>
            {
                WasSuccessful = false,
                Message = "No se pudo crear el usuario.",
            };
        }

        public async Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
        {
            return await _userRepository.AddUserToRoleAsync(user, roleName);
        }

        public Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return _userRepository.ConfirmEmailAsync(user, token);
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return _userRepository.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var ListUsers = await _userRepository.GetAllUsers();
            return _mapper.Map<List<UserDto>>(ListUsers);
        }

        public Task<User> GetUserAsync(string username)
        {
            return _userRepository.GetUserAsync(username);
        }

        public Task<User> GetUserByIdAsync(Guid userId)
        {
            return _userRepository.GetUserByIdAsync(userId);
        }

        public Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return _userRepository.IsUserInRoleAsync(user, roleName);
        }

        public async Task<TokenDto> LoginAsync(LoginUserDto loginUserDto)
        {
            SignInResult userlogin = await _userRepository.LoginAsync(_mapper.Map<User>(loginUserDto), loginUserDto.Password);
            if (userlogin.Succeeded)
            {
                User user = await _userRepository.FindByAlternateKeyAsync(x => x.UserName.ToLower().Equals(loginUserDto.UserName.ToLower().Trim()), string.Empty);
                List<string> roles = await _userRepository.GetUserRolesAsync(user);
                List<string> permissions = await _rolePermissionRepository.GetRolesPermissionsAsync(roles);
                return BuildToken(user, roles, permissions);
            }
            else
            {
                return null;
            }
        }

        public Task LogoutAsync()
        {
            return _userRepository.LogoutAsync();
        }

        public async Task<ActionResponse<bool>> RemoveUserFromRoleAsync(Guid userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccessful = false,
                        Message = "Usuario no encontrado."
                    };
                }

                user.IsDelete = true;

                var updateResult = await _userRepository.UpdateUserAsync(user);

                if (updateResult.Succeeded)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccessful = true,
                        Message = "El usuario se elimino correctamente.",
                        Result = true
                    };
                }

                return new ActionResponse<bool>
                {
                    WasSuccessful = false,
                    Message = "Usuario no fue eliminado, por favor intentelo nuevamente.",
                    Result = false
                };
                //var roles = await _userRepository.GetUserRolesAsync(user);
                //if (roles.Count != 0)
                //{
                //    var roleResult = await _userRepository.RemoveUserFromRoleAsync(user, roles[0]);
                //    if (!roleResult.Succeeded)
                //    {
                //        return new ActionResponse<bool>
                //        {
                //            WasSuccessful = false,
                //            Message = "Error al eliminar la relación con los roles del usuario.",
                //        };
                //    }
                //}

                //var deleteResult = await _userRepository.RemoveUserAsync(user);
                //if (!deleteResult.Succeeded)
                //{
                //    return new ActionResponse<bool>
                //    {
                //        WasSuccessful = false,
                //        Message = "Error al eliminar el usuario.",
                //    };
                //}
            }
            catch (Exception ex)
            {
                return new ActionResponse<bool>
                {
                    WasSuccessful = false,
                    Message = $"Ocurrió un error al intentar eliminar el usuario. {ex.Message}",
                };
            }
        }

        public async Task<ActionResponse<IdentityResult>> UpdateUserAsync(UserDto userDto)
        {
            var userFind = await GetUserAsync(userDto.DNI);
            if (userFind == null)
            {
                return new ActionResponse<IdentityResult>
                {
                    WasSuccessful = false,
                    Message = "El usuario no fue encontrado.",
                };
            }

            userFind.FirstName = userDto.FirstName ?? userDto.FirstName;
            userFind.LastName = userDto.LastName ?? userDto.LastName;
            userFind.Email = userDto.Email ?? userDto.Email;
            userFind.UserName = userDto.DNI ?? userDto.DNI;
            userFind.PhoneNumber = userDto.PhoneNumber ?? userDto.PhoneNumber;
            userFind.HireTypeId = userDto.HireTypeId;
            userFind.NickName = userDto.NickName;
            userFind.IsActive = (bool)userDto.IsActive;
            userFind.LawApply = userDto.LawApply;
            userFind.HiredHours = userDto.HiredHours;
            userFind.ComplementHours = userDto.ComplementHours;
            userFind.ExtraHours = userDto.ExtraHours;
            userFind.CantPartTimeSchedule = userDto.CantPartTimeSchedule;

            if (userDto.HireDate != null)
                userFind.HireDate = (DateTime)(userDto.HireDate != null ? userDto.HireDate : DateTime.UtcNow);

            var updateResult = await _userRepository.UpdateUserAsync(userFind);

            if (updateResult.Succeeded)
            {
                return new ActionResponse<IdentityResult>
                {
                    WasSuccessful = true,
                    Result = updateResult,
                    Message = "El usuario se actualizó correctamente."
                };
            }

            return new ActionResponse<IdentityResult>
            {
                WasSuccessful = false,
                Message = "No se pudo actualizar el usuario.",
            };
        }

        private TokenDto BuildToken(User user, List<string> roles, List<string> permissions)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("LastName", user.LastName),
                new Claim("UserName", user.UserName),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("Name", user.FirstName),
            };

            claims.Add(new Claim("Role", string.Join(",", roles)));
            claims.Add(new Claim("Permissions", string.Join(",", permissions)));

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiration = DateTime.UtcNow.AddHours(1);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new TokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}