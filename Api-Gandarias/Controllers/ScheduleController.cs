using CC.Domain.Dtos;
using CC.Domain.Enums;
using CC.Domain.Helpers;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IEmailService _emailService;
    private readonly IQrCodeService _qrCodeService;

    public ScheduleController(IScheduleService scheduleService, IEmailService emailService, IQrCodeService qrCodeService)
    {
        _scheduleService = scheduleService;
        _emailService = emailService;
        _qrCodeService = qrCodeService;
    }

    /// <summary>
    /// GET api/Schedule?fechaIni=2025-07-01
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(DateOnly fechaIni)
    {
        var userRole = User.GetRoles();
        Guid? userId = null;
        if (!userRole.Any(x => x == RoleType.Admin.ToString()))
        {
            userId = User.GetUserId();
        }

        DateOnly fechaFin = fechaIni.AddDays(6);
        return Ok(await _scheduleService.GetAllAsync(x => !x.IsDeleted &&
        x.Date >= fechaIni &&
        x.Date <= fechaFin &&
        (!userId.HasValue || x.UserId == userId), includeProperties: "User,Workstation.WorkArea").ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/Schedule?fechaIni=2025-07-01
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("ByUserId")]
    public async Task<IActionResult> GetByIdAsync(DateOnly fechaIni, Guid UserId)
    {
        DateOnly fechaFin = fechaIni.AddDays(6);
        return Ok(await _scheduleService.GetAllAsync(x => !x.IsDeleted &&
        x.Date >= fechaIni &&
        x.Date <= fechaFin &&
        x.UserId == UserId, includeProperties: "User,Workstation.WorkArea").ConfigureAwait(false));
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
            x.WorkstationId == scheduleDto.WorkstationId
            &&
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

    /// <summary>
    /// POST api/Schedule/notify?fechaIni=2025-07-01&fechaFin=2025-07-31
    /// </summary>
    /// <param name="fechaIni"></param>
    /// <param name="fechaFin"></param>
    /// <returns></returns>
    [HttpPost("notify")]
    public async Task<IActionResult> NotifySchedules(DateOnly fechaIni, DateOnly fechaFin)
    {
        try
        {
            var userRole = User.GetRoles();
            if (!userRole.Any(x => x == RoleType.Admin.ToString()))
            {
                return Unauthorized("Solo los administradores pueden notificar horarios.");
            }
            if (fechaIni > fechaFin)
            {
                return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin.");
            }

            var schedules = await _scheduleService.GetAllAsync(x =>
                !x.IsDeleted &&
                x.Date >= fechaIni &&
                x.Date <= fechaFin, includeProperties: "User,Workstation"
            ).ConfigureAwait(false);

            if (!schedules.Any())
            {
                return BadRequest("No hay horiarios programados para reportar");
            }

            var subject = $"Horarios programados del {fechaIni:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}";
            var groupedByUser = schedules.GroupBy(s => s.UserId);

            var tasks = groupedByUser.Select(
                async userGroup =>
                {
                    var user = userGroup.FirstOrDefault();
                    var body = HTMLHelper.GenerateScheduleHtml(userGroup.ToList());

                    var qrCode = await _qrCodeService.GenerateUserQrAsync(user.UserId).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(user?.UserEmail))
                    {
                        await _emailService.SendEmailAsync(user.UserEmail, subject, body, qrCode, $"{user.UserNickName}.png", "image/png");
                    }
                });

            await Task.WhenAll(tasks);

            return Ok("Horarios notificados correctamente.");
        }
        catch (Exception Ex)
        {
            return BadRequest("Error al notificar horarios: " + Ex.Message);
        }
    }
}