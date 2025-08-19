using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
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
        return Ok(await _workstationDemandService.GetAllAsync(includeProperties: "Workstation.WorkArea").ConfigureAwait(false));
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
        var data = await _workstationDemandService.GetAllAsync(x => x.TemplateId == id).ConfigureAwait(false);
        var result = data.Select(ToRawInput).ToList();
        return Ok(result);
    }

    /// <summary>
    /// POST api/WorkstationDemand
    /// </summary>
    /// <param name="WorkstationDemandDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(List<DemandInsertUpdateDto> workstationDemandDto)
    {
        var groupedByDay = workstationDemandDto
        .GroupBy(x => new { x.templateId, x.day })
        .ToList();

        foreach (var group in groupedByDay)
        {
            var existingData = await _workstationDemandService
                .GetAllAsync(x => x.TemplateId == group.Key.templateId && x.Day == group.Key.day)
                .ConfigureAwait(false);

            if (existingData?.Count() > 0)
            {
                await _workstationDemandService.DeleteRangeAsync(existingData);
            }
        }

        var dto = workstationDemandDto.Select(FromRawInput).ToList();
        await _workstationDemandService.AddRangeAsync(dto);
        return Ok(workstationDemandDto);
    }

    /// <summary>
    /// PUT api/WorkstationDemand
    /// </summary>
    /// <param name="WorkstationDemandDto"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Put(DemandCloneDto cloneDto)
    {
        var data = await _workstationDemandService.GetAllAsync(x => x.TemplateId == cloneDto.templateId && x.Day == cloneDto.day).ConfigureAwait(false);

        if (data == null || !data.Any())
        {
            return BadRequest("No hay información generada para clonar");
        }

        List<WorkstationDemandDto> cloneData = new List<WorkstationDemandDto>();
        foreach (var day in cloneDto.dayOfWeeks)
        {
            var dataDay = await _workstationDemandService.GetAllAsync(x => x.TemplateId == cloneDto.templateId && x.Day == day).ConfigureAwait(false);

            if (dataDay != null && dataDay.Any())
            {
                await _workstationDemandService.DeleteRangeAsync(dataDay);
            }
            foreach (var d in data)
            {
                var insertData = new WorkstationDemandDto()
                {
                    Day = day,
                    TemplateId = d.TemplateId,
                    EffortRequired = d.EffortRequired,
                    EndTime = d.EndTime,
                    StartTime = d.StartTime,
                    WorkstationId = d.WorkstationId,
                };
                cloneData.Add(insertData);
            }
        }
        await _workstationDemandService.AddRangeAsync(cloneData);
        return Ok(cloneDto);
    }

    /// <summary>
    /// PUT api/WorkstationDemand/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="WorkstationDemandDto"></param>
    /// <returns></returns>
    //[HttpPut]
    //public async Task<IActionResult> Put(List<DemandInsertUpdateDto> workstationDemandDto)
    //{
    //    var dto = workstationDemandDto.Select(FromRawInput).ToList();
    //    await _workstationDemandService.UpdateRangeAsync(dto);
    //    return Ok(workstationDemandDto);
    //}

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

    public static WorkstationDemandDto FromRawInput(DemandInsertUpdateDto demandInsertUpdateDto)
    {
        var startTime = new TimeSpan(demandInsertUpdateDto.hora, demandInsertUpdateDto.fraccion, 0);
        var endTime = startTime.Add(TimeSpan.FromMinutes(15));

        return new WorkstationDemandDto
        {
            Id = demandInsertUpdateDto.Id,
            WorkstationId = demandInsertUpdateDto.workstationId,
            TemplateId = demandInsertUpdateDto.templateId,
            Day = demandInsertUpdateDto.day,
            StartTime = startTime,
            EndTime = endTime,
            EffortRequired = demandInsertUpdateDto.effortRequired,
            WorkstationWorkAreaId = demandInsertUpdateDto.WorkstationWorkAreaId,
        };
    }

    public static DemandInsertUpdateDto ToRawInput(WorkstationDemandDto dto)
    {
        return new DemandInsertUpdateDto
        {
            Id = dto.Id,
            workstationId = dto.WorkstationId,
            templateId = dto.TemplateId,
            day = dto.Day,
            hora = dto.StartTime.Hours,
            fraccion = dto.StartTime.Minutes,
            effortRequired = dto.EffortRequired,
            WorkstationWorkAreaId = dto.WorkstationWorkAreaId
        };
    }
}