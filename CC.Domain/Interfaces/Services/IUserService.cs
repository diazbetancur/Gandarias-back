using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Helpers;
using Microsoft.AspNetCore.Identity;

namespace CC.Domain.Interfaces.Services
{
    public interface IUserService : IServiceBase<User, UserDto>
    {
        Task<TokenDto> LoginAsync(LoginUserDto loginUserDto);

        Task<ActionResponse<User>> AddUserAsync(UserDto user, string password);

        Task<ActionResponse<IdentityResult>> UpdateUserAsync(UserDto user);

        Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);

        Task<User> GetUserAsync(string username);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task LogoutAsync();

        Task<User> GetUserByIdAsync(Guid userId);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<ActionResponse<bool>> RemoveUserFromRoleAsync(Guid userId);

        Task<List<UserDto>> GetAllUsers();

        Task<ActionResponse<string>> GeneratePasswordResetTokenAsync(string userName);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}