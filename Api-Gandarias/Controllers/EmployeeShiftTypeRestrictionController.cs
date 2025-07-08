using CC.Application.Services;
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
public class EmployeeShiftTypeRestrictionController : ControllerBase
{
    private readonly IEmployeeShiftTypeRestrictionService _employeeShiftTypeRestrictionService;

    public EmployeeShiftTypeRestrictionController(IEmployeeShiftTypeRestrictionService employeeShiftTypeRestrictionService)
    {
        _employeeShiftTypeRestrictionService = employeeShiftTypeRestrictionService;
    }

    /// <summary>
    /// GET api/EmployeeShiftTypeRestriction
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _employeeShiftTypeRestrictionService
            .GetAllAsync(includeProperties: "User,ShiftType")
            .ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/EmployeeShiftTypeRestriction/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _employeeShiftTypeRestrictionService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/EmployeeShiftTypeRestriction/GetByUserId/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetByUserId/{id}")]
    public async Task<IActionResult> GetByUserId(Guid id)
    {
        return Ok(await _employeeShiftTypeRestrictionService.GetAllAsync(x => x.UserId == id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/EmployeeShiftTypeRestriction
    /// </summary>
    /// <param name="EmployeeShiftTypeRestrictionDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(List<EmployeeShiftTypeRestrictionDto> employeeShiftTypeRestrictionDto)
    {
        try
        {
            foreach (var item in employeeShiftTypeRestrictionDto)
            {
                var exist = await _employeeShiftTypeRestrictionService
                    .GetAllAsync(x => x.UserId == item.UserId && x.ShiftTypeId == item.ShiftTypeId)
                    .ConfigureAwait(false);

                if (exist.Any())
                    continue;

                await _employeeShiftTypeRestrictionService.AddAsync(item).ConfigureAwait(false);
            }

            return Ok(employeeShiftTypeRestrictionDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// PUT api/EmployeeShiftTypeRestriction/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="EmployeeShiftTypeRestrictionDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(EmployeeShiftTypeRestrictionDto employeeShiftTypeRestrictionDto)
    {
        await _employeeShiftTypeRestrictionService.UpdateAsync(employeeShiftTypeRestrictionDto).ConfigureAwait(false);
        return Ok(employeeShiftTypeRestrictionDto);
    }

    /// <summary>
    /// DELETE api/EmployeeShiftTypeRestriction
    /// </summary>
    /// <param name="EmployeeShiftTypeRestrictionDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(EmployeeShiftTypeRestrictionDto employeeShiftTypeRestrictionDto)
    {
        await _employeeShiftTypeRestrictionService.DeleteAsync(employeeShiftTypeRestrictionDto).ConfigureAwait(false);
        return Ok(employeeShiftTypeRestrictionDto);
    }
}