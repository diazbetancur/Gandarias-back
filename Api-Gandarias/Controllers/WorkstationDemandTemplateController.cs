using CC.Application.Services;
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
public class WorkstationDemandTemplateController : ControllerBase
{
    private readonly IWorkstationDemandTemplateService _workstationDemandTemplateService;

    public WorkstationDemandTemplateController(IWorkstationDemandTemplateService workstationDemandTemplateService)
    {
        _workstationDemandTemplateService = workstationDemandTemplateService;
    }

    /// <summary>
    /// GET api/WorkstationDemandTemplate
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _workstationDemandTemplateService.GetAllAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/WorkstationDemandTemplate/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _workstationDemandTemplateService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/WorkstationDemandTemplate
    /// </summary>
    /// <param name="WorkstationDemandTemplateDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(WorkstationDemandTemplateDto workstationDemandTemplateDto)
    {
        try
        {
            await _workstationDemandTemplateService.AddAsync(workstationDemandTemplateDto).ConfigureAwait(false);
            return Ok(workstationDemandTemplateDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// PUT api/WorkstationDemandTemplate/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="WorkstationDemandTemplateDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, WorkstationDemandTemplateDto workstationDemandTemplateDto)
    {
        workstationDemandTemplateDto.Id = id;
        await _workstationDemandTemplateService.UpdateAsync(workstationDemandTemplateDto).ConfigureAwait(false);
        return Ok(workstationDemandTemplateDto);
    }

    /// <summary>
    /// DELETE api/WorkstationDemandTemplate/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="WorkstationDemandTemplateDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(WorkstationDemandTemplateDto workstationDemandTemplateDto)
    {
        await _workstationDemandTemplateService.DeleteAsync(workstationDemandTemplateDto).ConfigureAwait(false);
        return Ok(workstationDemandTemplateDto);
    }
}