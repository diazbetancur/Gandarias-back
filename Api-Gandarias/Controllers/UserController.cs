using CC.Domain.Dtos;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Gandarias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginUserDto loginUserDto)
        {
            TokenDto result = await _userService.LoginAsync(loginUserDto);
            return result != null ? Ok(result) : StatusCode((int)HttpStatusCode.Unauthorized);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet()]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _userService.GetAllUsers().ConfigureAwait(false));
        }

        /// <summary>
        /// GET api/user/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            return Ok(await _userService.GetUserAsync(id).ConfigureAwait(false));
        }

        /// <summary>
        /// POST api/user
        /// </summary>
        /// <param name="UserDto"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Post(UserDto userDTO)
        {
            await _userService.AddUserAsync(userDTO, userDTO.Password).ConfigureAwait(false);
            return Ok(userDTO);
        }

        /// <summary>
        /// PUT api/user
        /// </summary>
        /// returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> Put(UserDto userDTO)
        {
            await _userService.UpdateUserAsync(userDTO).ConfigureAwait(false);
            return Ok(userDTO);
        }

        /// <summary>
        /// DELETE api/user/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
        /// </summary>
        /// returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            await _userService.RemoveUserFromRoleAsync(userId).ConfigureAwait(false);
            return Ok();
        }
    }
}