﻿using CC.Domain.Dtos;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EmployeeScheduleRestrictionController : ControllerBase
{
    private readonly IEmployeeScheduleRestrictionService _employeeScheduleRestriction;

    public EmployeeScheduleRestrictionController(IEmployeeScheduleRestrictionService employeeScheduleRestrictionService)
    {
        _employeeScheduleRestriction = employeeScheduleRestrictionService;
    }

    /// <summary>
    /// GET api/EmployeeScheduleRestriction
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _employeeScheduleRestriction.GetAllAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/EmployeeScheduleRestriction/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _employeeScheduleRestriction.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/EmployeeScheduleRestriction/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetByUserId/{id}")]
    public async Task<IActionResult> GetByUserId(Guid id)
    {
        return Ok(await _employeeScheduleRestriction.GetAllAsync(x => x.UserId == id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/EmployeeScheduleRestriction
    /// </summary>
    /// <param name="EmployeeScheduleRestrictionDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(List<EmployeeScheduleRestrictionDto> employeeScheduleRestrictionDto)
    {
        foreach (var item in employeeScheduleRestrictionDto)
        {
            if (string.IsNullOrEmpty(item.Id.ToString().Trim()))
            {
                await _employeeScheduleRestriction.AddAsync(item).ConfigureAwait(false);
            }
            else
            {
                await _employeeScheduleRestriction.UpdateAsync(item).ConfigureAwait(false);
            }
        }

        return Ok();
    }

    /// <summary>
    /// DELETE api/EmployeeScheduleRestriction/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="EmployeeScheduleRestrictionDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(EmployeeScheduleRestrictionDto employeeScheduleRestrictionDto)
    {
        await _employeeScheduleRestriction.DeleteAsync(employeeScheduleRestrictionDto).ConfigureAwait(false);
        return Ok(employeeScheduleRestrictionDto);
    }
}