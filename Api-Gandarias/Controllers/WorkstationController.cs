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
public class WorkstationController : ControllerBase
{
    private readonly IWorkstationService _workstationService;

    public WorkstationController(IWorkstationService workstationService)
    {
        _workstationService = workstationService;
    }

    /// <summary>
    /// GET api/Workstation
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _workstationService.GetAllAsync(includeProperties: "WorkArea").ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/Workstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _workstationService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/Workstation
    /// </summary>
    /// <param name="workstationDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(WorkstationDto workstationDto)
    {
        await _workstationService.AddAsync(workstationDto).ConfigureAwait(false);
        return Ok(workstationDto);
    }

    /// <summary>
    /// PUT api/Workstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="workstationDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, WorkstationDto workstationDto)
    {
        workstationDto.Id = id;
        await _workstationService.UpdateAsync(workstationDto).ConfigureAwait(false);
        return Ok(workstationDto);
    }

    /// <summary>
    /// DELETE api/Workstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="workstationDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(WorkstationDto workstationDto)
    {
        workstationDto.IsActive = false;
        await _workstationService.UpdateAsync(workstationDto).ConfigureAwait(false);
        return Ok(workstationDto);
    }
}