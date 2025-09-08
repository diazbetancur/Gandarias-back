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
public class EmployeeScheduleExceptionController : ControllerBase
{
    private readonly IEmployeeScheduleExceptionService _employeeScheduleExceptionService;

    public EmployeeScheduleExceptionController(IEmployeeScheduleExceptionService employeeScheduleExceptionService)
    {
        _employeeScheduleExceptionService = employeeScheduleExceptionService;
    }

    /// <summary>
    /// GET api/EmployeeScheduleException
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-365));
        var rawData = await _employeeScheduleExceptionService.GetAllAsync(x => x.Date >= fromDate, includeProperties: "User").ConfigureAwait(false);

        var formatted = rawData.Select(e => new EmployeeScheduleExceptionResponseDto
        {
            Id = e.Id,
            UserId = e.UserId,
            UserFullName = e.UserFullName,
            Date = e.Date.ToString("dd/MM/yyyy"),
            RestrictionType = e.RestrictionType,
            IsAdditionalRestriction = e.IsAdditionalRestriction,
            AvailableFrom = e.AvailableFrom,
            AvailableUntil = e.AvailableUntil,
            Block1Start = e.Block1Start,
            Block1End = e.Block1End,
            Block2Start = e.Block2Start,
            Block2End = e.Block2End
        });

        return Ok(formatted);
    }

    /// <summary>
    /// GET api/EmployeeScheduleException/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _employeeScheduleExceptionService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/EmployeeScheduleException/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetByUserId/{id}")]
    public async Task<IActionResult> GetByUserId(Guid id)
    {
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-365));
        return Ok(await _employeeScheduleExceptionService.GetAllAsync(x => x.UserId == id && x.Date >= fromDate).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/EmployeeScheduleException
    /// </summary>
    /// <param name="EmployeeScheduleExceptionDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(EmployeeScheduleExceptionDto employeeScheduleExceptionDto)
    {
        var response = await _employeeScheduleExceptionService.GetAllAsync(x =>
            x.UserId == employeeScheduleExceptionDto.UserId &&
            x.Date == employeeScheduleExceptionDto.Date
        ).ConfigureAwait(false);

        if (response.Count() > 0)
        {
            return BadRequest("Ya existe una novedad para esa fecha.");
        }

        await _employeeScheduleExceptionService.AddAsync(employeeScheduleExceptionDto).ConfigureAwait(false);
        return Ok(employeeScheduleExceptionDto);
    }

    /// <summary>
    /// PUT api/EmployeeScheduleException/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="EmployeeScheduleExceptionDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, EmployeeScheduleExceptionDto employeeScheduleExceptionDto)
    {
        employeeScheduleExceptionDto.Id = id;

        var response = await _employeeScheduleExceptionService.GetAllAsync(x =>
    x.UserId == employeeScheduleExceptionDto.UserId &&
    x.Date == employeeScheduleExceptionDto.Date &&
    x.Id != employeeScheduleExceptionDto.Id
).ConfigureAwait(false);

        if (response.Count() > 0)
        {
            return BadRequest("Ya existe una novedad para esa fecha.");
        }

        await _employeeScheduleExceptionService.UpdateAsync(employeeScheduleExceptionDto).ConfigureAwait(false);
        return Ok(employeeScheduleExceptionDto);
    }

    /// <summary>
    /// DELETE api/EmployeeScheduleException/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="EmployeeScheduleExceptionDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(EmployeeScheduleExceptionDto employeeScheduleExceptionDto)
    {
        try
        {
            await _employeeScheduleExceptionService.DeleteAsync(employeeScheduleExceptionDto).ConfigureAwait(false);
            return Ok(employeeScheduleExceptionDto);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar la novedad");
        }
    }
}