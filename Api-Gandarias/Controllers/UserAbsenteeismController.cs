using CC.Domain.Dtos;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserAbsenteeismController : ControllerBase
{
    private readonly IUserAbsenteeismService _userAbsenteeismService;

    public UserAbsenteeismController(IUserAbsenteeismService userAbsenteeismService)
    {
        _userAbsenteeismService = userAbsenteeismService;
    }

    /// <summary>
    /// GET api/UserAbsenteeism
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _userAbsenteeismService.GetAllAsync(includeProperties: "User,AbsenteeismType").ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/UserAbsenteeism/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _userAbsenteeismService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/UserAbsenteeism/GetByUserId/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetByUserId/{id}")]
    public async Task<IActionResult> GetByUserId(Guid id)
    {
        return Ok(await _userAbsenteeismService.GetAllAsync(x => x.UserId == id, includeProperties: "User,AbsenteeismType").ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/UserAbsenteeism
    /// </summary>
    /// <param name="UserAbsenteeismDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(UserAbsenteeismDto userAbsenteeismDto)
    {
        var existingAbsenteeism = await _userAbsenteeismService.GetAllAsync(
            x => x.UserId == userAbsenteeismDto.UserId &&
                 (x.EndDate ?? DateTime.MaxValue) >= userAbsenteeismDto.StartDate &&
                 x.StartDate <= (userAbsenteeismDto.EndDate ?? DateTime.MaxValue)
        ).ConfigureAwait(false);
        if (existingAbsenteeism.Any())
        {
            return BadRequest("Ya existe una novedad para el periodo seleccionado.");
        }
        await _userAbsenteeismService.AddAsync(userAbsenteeismDto).ConfigureAwait(false);
        return Ok(userAbsenteeismDto);
    }

    /// <summary>
    /// PUT api/UserAbsenteeism/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="UserAbsenteeismDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UserAbsenteeismDto userAbsenteeismDto)
    {
        userAbsenteeismDto.Id = id;
        var existingAbsenteeism = await _userAbsenteeismService.GetAllAsync(
            x => x.UserId == userAbsenteeismDto.UserId &&
            x.Id != id &&
                 (x.EndDate ?? DateTime.MaxValue) >= userAbsenteeismDto.StartDate &&
                 x.StartDate <= (userAbsenteeismDto.EndDate ?? DateTime.MaxValue)
        ).ConfigureAwait(false);
        if (existingAbsenteeism.Any())
        {
            return BadRequest("Ya existe una novedad para el periodo seleccionado.");
        }
        await _userAbsenteeismService.UpdateAsync(userAbsenteeismDto).ConfigureAwait(false);
        return Ok(userAbsenteeismDto);
    }

    /// <summary>
    /// DELETE api/UserAbsenteeism/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="UserWorkstationDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(UserAbsenteeismDto userAbsenteeismDto)
    {
        await _userAbsenteeismService.DeleteAsync(userAbsenteeismDto).ConfigureAwait(false);
        return Ok(userAbsenteeismDto);
    }
}