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
    private readonly IWorkstationDemandService _workstationDemandService;

    public WorkstationDemandTemplateController(IWorkstationDemandTemplateService workstationDemandTemplateService, IWorkstationDemandService workstationDemandService)
    {
        _workstationDemandTemplateService = workstationDemandTemplateService;
        _workstationDemandService = workstationDemandService;
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
            //var templates = await _workstationDemandTemplateService.GetAllAsync().ConfigureAwait(false);

            //if (HasOverlappingDateRangeByMonthDay(templates, workstationDemandTemplateDto.StartDate, workstationDemandTemplateDto.EndDate))
            //{
            //    return BadRequest("Ya existe un rango que se cruza con el nuevo (por mes y día).");
            //}

            var response = await _workstationDemandTemplateService.AddAsync(workstationDemandTemplateDto).ConfigureAwait(false);
            return Ok(response);
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

        var templates = await _workstationDemandTemplateService
            .GetAllAsync(x => x.Id != id)
            .ConfigureAwait(false);

        //if (HasOverlappingDateRangeByMonthDay(templates, workstationDemandTemplateDto.StartDate, workstationDemandTemplateDto.EndDate))
        //{
        //    return BadRequest("Ya existe un rango que se cruza con el nuevo (por mes y día).");
        //}

        await _workstationDemandTemplateService.UpdateAsync(workstationDemandTemplateDto);

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
        var data = await _workstationDemandService.GetAllAsync(x => x.TemplateId == workstationDemandTemplateDto.Id).ConfigureAwait(false);
        await _workstationDemandService.DeleteRangeAsync(data).ConfigureAwait(false);
        await _workstationDemandTemplateService.DeleteAsync(workstationDemandTemplateDto).ConfigureAwait(false);
        return Ok(workstationDemandTemplateDto);
    }

    private bool HasOverlappingDateRangeByMonthDay(IEnumerable<WorkstationDemandTemplateDto> templates, DateOnly newStart, DateOnly newEnd)
    {
        var newRangeStart = new DateOnly(1, newStart.Month, newStart.Day);
        var newRangeEnd = new DateOnly(1, newEnd.Month, newEnd.Day);

        return templates.Any(t =>
        {
            var existingRangeStart = new DateOnly(1, t.StartDate.Month, t.StartDate.Day);
            var existingRangeEnd = new DateOnly(1, t.EndDate.Month, t.EndDate.Day);

            return RangesOverlap(existingRangeStart, existingRangeEnd, newRangeStart, newRangeEnd);
        });
    }

    private bool RangesOverlap(DateOnly start1, DateOnly end1, DateOnly start2, DateOnly end2)
    {
        return start1 <= end2 && start2 <= end1;
    }
}