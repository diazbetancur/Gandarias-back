using CC.Domain.Dtos;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class WorkAreaController : ControllerBase
{
    private readonly IWorkAreaService _workAreaService;

    public WorkAreaController(IWorkAreaService workAreaService)
    {
        _workAreaService = workAreaService;
    }

    /// <summary>
    /// GET api/WorkArea
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _workAreaService.GetAllAsync(x => x.IsActive).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/WorkArea/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _workAreaService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/WorkArea
    /// </summary>
    /// <param name="workAreaDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(WorkAreaDto workAreaDto)
    {
        await _workAreaService.AddAsync(workAreaDto).ConfigureAwait(false);
        return Ok(workAreaDto);
    }

    /// <summary>
    /// PUT api/WorkArea/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="workAreaDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, WorkAreaDto workAreaDto)
    {
        workAreaDto.Id = id;
        await _workAreaService.UpdateAsync(workAreaDto).ConfigureAwait(false);
        return Ok(workAreaDto);
    }

    /// <summary>
    /// DELETE api/WorkArea/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="workAreaDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(WorkAreaDto workAreaDto)
    {
        await _workAreaService.DeleteAsync(workAreaDto).ConfigureAwait(false);
        return Ok(workAreaDto);
    }
}