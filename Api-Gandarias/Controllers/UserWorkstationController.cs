using CC.Domain.Dtos;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserWorkstationController : ControllerBase
{
    private readonly IUserWorkstationService _userWorkstationService;

    public UserWorkstationController(IUserWorkstationService userWorkstationService)
    {
        _userWorkstationService = userWorkstationService;
    }

    /// <summary>
    /// GET api/UserWorkstation
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _userWorkstationService.GetAllAsync(x => x.IsDelete == false).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/UserWorkstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _userWorkstationService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/UserWorkstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetByUserId/{id}")]
    public async Task<IActionResult> GetByUserId(Guid id)
    {
        return Ok(await _userWorkstationService.GetAllAsync(x => x.UserId == id && x.IsDelete == false).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/UserWorkstation
    /// </summary>
    /// <param name="UserWorkstationDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(AddUserWorkstationDto userWorkstationDto)
    {
        foreach (var item in userWorkstationDto.workStations)
        {
            await _userWorkstationService.AddAsync(new UserWorkstationDto
            {
                Coverage = item.Coverage,
                UserId = userWorkstationDto.UserId,
                WorkstationId = item.Id,
                IsDelete = false
            }).ConfigureAwait(false);
        }
        return Ok(userWorkstationDto);
    }

    /// <summary>
    /// PUT api/UserWorkstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="UserWorkstationDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UserWorkstationDto userWorkstationDto)
    {
        userWorkstationDto.Id = id;
        await _userWorkstationService.UpdateAsync(userWorkstationDto).ConfigureAwait(false);
        return Ok(userWorkstationDto);
    }

    /// <summary>
    /// DELETE api/UserWorkstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="UserWorkstationDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(UserWorkstationDto userWorkstationDto)
    {
        userWorkstationDto.IsDelete = true;
        await _userWorkstationService.UpdateAsync(userWorkstationDto).ConfigureAwait(false);
        return Ok(userWorkstationDto);
    }
}