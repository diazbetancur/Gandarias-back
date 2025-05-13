using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserRestrictionShiftController : ControllerBase
{
    private readonly IUserRestrictionShiftService _userRestrictionShiftService;

    public UserRestrictionShiftController(IUserRestrictionShiftService userRestrictionShiftService)
    {
        _userRestrictionShiftService = userRestrictionShiftService;
    }

    /// <summary>
    /// GET api/UserWorkstation
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _userRestrictionShiftService
            .GetAllAsync(x => x.IsDelete == false, includeProperties: "User,ShiftType")
            .ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/UserRestrictionShift/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _userRestrictionShiftService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/UserRestrictionShift
    /// </summary>
    /// <param name="UserRestrictionShiftDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(UserRestrictionShiftDto userRestrictionShiftDto)
    {
        try
        {
            await _userRestrictionShiftService.AddAsync(userRestrictionShiftDto).ConfigureAwait(false);
            return Ok(userRestrictionShiftDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// PUT api/UserRestrictionShift/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="UserRestrictionShiftDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UserRestrictionShiftDto userRestrictionShiftDto)
    {
        userRestrictionShiftDto.Id = id;
        await _userRestrictionShiftService.UpdateAsync(userRestrictionShiftDto).ConfigureAwait(false);
        return Ok(userRestrictionShiftDto);
    }

    /// <summary>
    /// DELETE api/UserRestrictionShift/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="UserRestrictionShiftDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(UserRestrictionShiftDto userRestrictionShiftDto)
    {
        userRestrictionShiftDto.IsDelete = true;
        await _userRestrictionShiftService.UpdateAsync(userRestrictionShiftDto).ConfigureAwait(false);
        return Ok(userRestrictionShiftDto);
    }
}