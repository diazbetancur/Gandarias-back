using CC.Domain.Dtos;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserShiftController : ControllerBase
{
    private readonly IUserShiftService _userShiftService;

    public UserShiftController(IUserShiftService userShiftService)
    {
        _userShiftService = userShiftService;
    }

    /// <summary>
    /// GET api/UserShift
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _userShiftService.GetAllAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/UserShift/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _userShiftService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/UserShift/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetByUserId/{id}")]
    public async Task<IActionResult> GetByUserId(Guid id)
    {
        return Ok(await _userShiftService.GetAllAsync(x => x.UserId == id, includeProperties: "User").ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/UserShift
    /// </summary>
    /// <param name="userShiftDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(UserShiftDto userShiftDto)
    {
        await _userShiftService.AddAsync(userShiftDto).ConfigureAwait(false);
        return Ok(userShiftDto);
    }

    /// <summary>
    /// PUT api/UserShift/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userShiftDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UserShiftDto userShiftDto)
    {
        userShiftDto.Id = id;
        await _userShiftService.UpdateAsync(userShiftDto).ConfigureAwait(false);
        return Ok(userShiftDto);
    }

    /// <summary>
    /// DELETE api/UserShift/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="userShiftDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(UserShiftDto userShiftDto)
    {
        await _userShiftService.DeleteAsync(userShiftDto).ConfigureAwait(false);
        return Ok(userShiftDto);
    }
}