using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Helpers;
using CC.Domain.Interfaces.Repositories;
using CC.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Repositories
{
    public class UserRepository : ERepositoryBase<User>, IUserRepository
    {
        private readonly DBContext _dataContext;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserRepository(IQueryableUnitOfWork unitOfWork, SignInManager<User> signInManager, UserManager<User> userManager, DBContext dataContext) : base(unitOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dataContext = dataContext;
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<SignInResult> LoginAsync(User user, string password)
        {
            var respues = await _signInManager.PasswordSignInAsync(user.UserName, password, false, true);
            return respues;
        }

        public async Task<ActionResponse<User>> AddUserAsync(UserDto userDto, string password)
        {
            User newUser = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                UserName = userDto.DNI,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                NickName = userDto.NickName,
                HireDate = (DateTime)userDto.HireDate,
                HireTypeId = userDto.HireTypeId,
                IsDelete = false,
                IsActive = true
            };

            var response = await _userManager.CreateAsync(newUser, password);
            if (response.Succeeded)
            {
                return new ActionResponse<User>
                {
                    WasSuccessful = true,
                    Result = newUser
                };
            }
            else
            {
                return new ActionResponse<User>
                {
                    WasSuccessful = false,
                    Message = response.Errors.FirstOrDefault().Description
                };
            }
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
        {
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<User> GetUserAsync(string username)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.UserName == username);
            return user;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(string username, string password)
        {
            return await _signInManager.PasswordSignInAsync(username, password, false, true);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            return user;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(User user, string roleName)
        {
            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveUserAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            {
                var listUsers = await _dataContext.Users.Where(x => x.IsDelete == false).ToListAsync();
                var userDtos = new List<UserDto>();

                foreach (var user in listUsers)
                {
                    var hireType = await _dataContext.HireTypes.FindAsync(user.HireTypeId);

                    var rolNames = await _dataContext.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Join(_dataContext.Roles,
                              ur => ur.RoleId,
                              r => r.Id,
                              (ur, r) => r.Name)
                        .ToListAsync();

                    var userDto = new UserDto
                    {
                        Id = user.Id,
                        DNI = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        HireTypeId = user.HireTypeId,
                        HireDate = user.HireDate,
                        IsActive = user.IsActive,
                        NickName = user.NickName,
                        HireTypeName = hireType?.Name,
                        RolName = string.Join(", ", rolNames)
                    };

                    userDtos.Add(userDto);
                }

                return userDtos;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Trim() == email.ToLower().Trim());
            return user;
        }
    }
}