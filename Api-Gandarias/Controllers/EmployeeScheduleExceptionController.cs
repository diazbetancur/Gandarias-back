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
        return Ok(await _employeeScheduleExceptionService.GetAllAsync(includeProperties: "User").ConfigureAwait(false));
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
        return Ok(await _employeeScheduleExceptionService.GetAllAsync(x => x.UserId == id).ConfigureAwait(false));
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
        await _employeeScheduleExceptionService.DeleteAsync(employeeScheduleExceptionDto).ConfigureAwait(false);
        return Ok(employeeScheduleExceptionDto);
    }
}