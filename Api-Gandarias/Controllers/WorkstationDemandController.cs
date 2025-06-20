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
public class WorkstationDemandController : ControllerBase
{
    private readonly IWorkstationDemandService _workstationDemandService;

    public WorkstationDemandController(IWorkstationDemandService workstationDemandService)
    {
        _workstationDemandService = workstationDemandService;
    }

    /// <summary>
    /// GET api/WorkstationDemand
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _workstationDemandService.GetAllAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/WorkstationDemand/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _workstationDemandService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/WorkstationDemand/WorkstationDemandTemplateId/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("WorkstationDemandTemplateId/{id}")]
    public async Task<IActionResult> GetByWorkstationDemandTemplateIdAsync(Guid id)
    {
        return Ok(await _workstationDemandService.GetAllAsync(x => x.TemplateId == id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/WorkstationDemand
    /// </summary>
    /// <param name="WorkstationDemandDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(WorkstationDemandDto workstationDemandDto)
    {
        try
        {
            var response = await _workstationDemandService.GetAllAsync(x =>
                x.WorkstationId == workstationDemandDto.WorkstationId &&
                x.TemplateId == workstationDemandDto.TemplateId &&
                x.Day == workstationDemandDto.Day
            ).ConfigureAwait(false);

            bool isOverlapping = response.Any(existing =>
                workstationDemandDto.StartTime < existing.EndTime &&
                workstationDemandDto.EndTime > existing.StartTime
            );

            if (isOverlapping)
            {
                return BadRequest("Ya existe un horario que se sobrepone con el seleccionado.");
            }

            await _workstationDemandService.AddAsync(workstationDemandDto).ConfigureAwait(false);
            return Ok(workstationDemandDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// PUT api/WorkstationDemand/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="WorkstationDemandDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, WorkstationDemandDto workstationDemandDto)
    {
        workstationDemandDto.Id = id;

        var response = await _workstationDemandService.GetAllAsync(x =>
                x.WorkstationId == workstationDemandDto.WorkstationId &&
                x.TemplateId == workstationDemandDto.TemplateId &&
                x.Day == workstationDemandDto.Day
            ).ConfigureAwait(false);

        bool isOverlapping = response.Any(existing =>
            workstationDemandDto.StartTime < existing.EndTime &&
            workstationDemandDto.EndTime > existing.StartTime
        );

        if (isOverlapping)
        {
            return BadRequest("Ya existe un horario que se sobrepone con el seleccionado.");
        }

        await _workstationDemandService.UpdateAsync(workstationDemandDto).ConfigureAwait(false);
        return Ok(workstationDemandDto);
    }

    /// <summary>
    /// DELETE api/WorkstationDemand/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="WorkstationDemandDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(WorkstationDemandDto workstationDemandDto)
    {
        await _workstationDemandService.DeleteAsync(workstationDemandDto).ConfigureAwait(false);
        return Ok(workstationDemandDto);
    }
}