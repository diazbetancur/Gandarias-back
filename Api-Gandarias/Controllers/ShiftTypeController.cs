using CC.Application.Services;
using CC.Domain.Dtos;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ShiftTypeController : ControllerBase
{
    private readonly IShiftTypeService _shiftTypeService;

    public ShiftTypeController(IShiftTypeService shiftTypeService)
    {
        _shiftTypeService = shiftTypeService;
    }

    /// <summary>
    /// GET api/ShiftType
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _shiftTypeService.GetAllAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/ShiftType/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _shiftTypeService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/ShiftType
    /// </summary>
    /// <param name="shiftTypeDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(ShiftTypeDto shiftTypeDto)
    {
        shiftTypeDto.IsActive = true;
        await _shiftTypeService.AddAsync(shiftTypeDto).ConfigureAwait(false);
        return Ok(shiftTypeDto);
    }

    /// <summary>
    /// PUT api/ShiftType/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="shiftTypeDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, ShiftTypeDto shiftTypeDto)
    {
        shiftTypeDto.Id = id;
        await _shiftTypeService.UpdateAsync(shiftTypeDto).ConfigureAwait(false);
        return Ok(shiftTypeDto);
    }

    /// <summary>
    /// DELETE api/ShiftType/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="shiftTypeDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(ShiftTypeDto shiftTypeDto)
    {
        await _shiftTypeService.DeleteAsync(shiftTypeDto).ConfigureAwait(false);
        return Ok(shiftTypeDto);
    }
}