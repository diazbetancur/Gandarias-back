using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Helpers;
using Microsoft.AspNetCore.Identity;

namespace CC.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IERepositoryBase<User>
    {
        Task<SignInResult> LoginAsync(User user, string password);

        Task<List<string>> GetUserRolesAsync(User user);

        Task<ActionResponse<User>> AddUserAsync(UserDto userDto, string password);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);

        Task<User> GetUserAsync(string username);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task LogoutAsync();

        Task<User> GetUserByIdAsync(Guid userId);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<IdentityResult> RemoveUserFromRoleAsync(User user, string roleName);

        Task<IdentityResult> RemoveUserAsync(User user);

        Task<List<UserDto>> GetAllUsers();

        Task<User> GetUserByEmailAsync(string email);
    }
}