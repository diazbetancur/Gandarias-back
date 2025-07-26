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
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    /// <summary>
    /// GET api/Schedule?userId={userId}&fechaIni=2025-07-01&fechaFin=2025-07-31
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(Guid? userId, DateOnly fechaIni, DateOnly fechaFin)
    {
        return Ok(await _scheduleService.GetAllAsync(x => !x.IsDeleted &&
        x.Date >= fechaIni &&
        x.Date <= fechaFin &&
        (!userId.HasValue || x.UserId == userId)).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/Schedule
    /// </summary>
    /// <param name="ScheduleDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(ScheduleDto scheduleDto)
    {
        if (scheduleDto.StartTime >= scheduleDto.EndTime)
            return BadRequest("La hora de inicio debe ser menor que la hora de fin.");

        var conflict = await _scheduleService.GetAllAsync(x =>
            !x.IsDeleted &&
            x.Date == scheduleDto.Date &&
            x.UserId == scheduleDto.UserId &&
            x.WorkstationId == scheduleDto.WorkstationId &&
            (
                (scheduleDto.StartTime >= x.StartTime && scheduleDto.StartTime < x.EndTime) ||
                (scheduleDto.EndTime > x.StartTime && scheduleDto.EndTime <= x.EndTime) ||
                (scheduleDto.StartTime <= x.StartTime && scheduleDto.EndTime >= x.EndTime)
            )
        ).ConfigureAwait(false);

        if (conflict.Any())
            return BadRequest("Ya existe un horario para este usuario con ese puesto de trabajo en ese rango de tiempo.");

        await _scheduleService.AddAsync(scheduleDto).ConfigureAwait(false);
        return Ok(scheduleDto);
    }

    /// <summary>
    /// PUT api/Schedule/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ScheduleDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, ScheduleDto scheduleDto)
    {
        if (scheduleDto.StartTime >= scheduleDto.EndTime)
            return BadRequest("La hora de inicio debe ser menor que la hora de fin.");

        var conflict = await _scheduleService.GetAllAsync(x =>
            !x.IsDeleted &&
            x.Id != id &&
            x.Date == scheduleDto.Date &&
            x.UserId == scheduleDto.UserId &&
            x.WorkstationId == scheduleDto.WorkstationId &&
            (
                (scheduleDto.StartTime >= x.StartTime && scheduleDto.StartTime < x.EndTime) ||
                (scheduleDto.EndTime > x.StartTime && scheduleDto.EndTime <= x.EndTime) ||
                (scheduleDto.StartTime <= x.StartTime && scheduleDto.EndTime >= x.EndTime)
            )
        ).ConfigureAwait(false);

        if (conflict.Any())
            return BadRequest("Ya existe un horario para este usuario con ese puesto de trabajo en ese rango de tiempo.");

        scheduleDto.Id = id;
        await _scheduleService.UpdateAsync(scheduleDto).ConfigureAwait(false);
        return Ok(scheduleDto);
    }

    /// <summary>
    /// DELETE api/Schedule/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="ScheduleDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(ScheduleDto scheduleDto)
    {
        scheduleDto.IsDeleted = true;
        await _scheduleService.DeleteAsync(scheduleDto).ConfigureAwait(false);
        return Ok(scheduleDto);
    }
}