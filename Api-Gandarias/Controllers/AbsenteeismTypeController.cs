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
public class AbsenteeismTypeController : ControllerBase
{
    private readonly IAbsenteeismTypeService _absenteeismTypeService;

    public AbsenteeismTypeController(IAbsenteeismTypeService absenteeismTypeService)
    {
        _absenteeismTypeService = absenteeismTypeService;
    }

    /// <summary>
    /// GET api/AbsenteeismType
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _absenteeismTypeService.GetAllAsync(x => !x.IsDeleted).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/AbsenteeismType/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _absenteeismTypeService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/AbsenteeismType
    /// </summary>
    /// <param name="AbsenteeismTypeDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(AbsenteeismTypeDto absenteeismTypeDto)
    {
        await _absenteeismTypeService.AddAsync(absenteeismTypeDto).ConfigureAwait(false);
        return Ok(absenteeismTypeDto);
    }

    /// <summary>
    /// PUT api/AbsenteeismType/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="AbsenteeismTypeDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, AbsenteeismTypeDto absenteeismTypeDto)
    {
        absenteeismTypeDto.Id = id;
        await _absenteeismTypeService.UpdateAsync(absenteeismTypeDto).ConfigureAwait(false);
        return Ok(absenteeismTypeDto);
    }

    /// <summary>
    /// DELETE api/AbsenteeismType/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="AbsenteeismTypeDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(AbsenteeismTypeDto absenteeismTypeDto)
    {
        absenteeismTypeDto.IsDeleted = true;
        await _absenteeismTypeService.UpdateAsync(absenteeismTypeDto).ConfigureAwait(false);
        return Ok(absenteeismTypeDto);
    }
}