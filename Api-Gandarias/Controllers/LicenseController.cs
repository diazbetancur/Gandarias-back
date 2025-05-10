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
public class LicenseController : ControllerBase
{
    private readonly ILicenseService _licenseService;

    public LicenseController(ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    /// <summary>
    /// GET api/License
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _licenseService.GetAllAsync(x => x.IsDeleted == false, includeProperties: "User").ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/License/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _licenseService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/License
    /// </summary>
    /// <param name="licenseDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(LicenseDto licenseDto)
    {
        try
        {
            await _licenseService.AddAsync(licenseDto).ConfigureAwait(false);
            return Ok(licenseDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// PUT api/License/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="licenseDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, LicenseDto licenseDto)
    {
        licenseDto.Id = id;
        await _licenseService.UpdateAsync(licenseDto).ConfigureAwait(false);
        return Ok(licenseDto);
    }

    /// <summary>
    /// DELETE api/License/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="licenseDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(LicenseDto licenseDto)
    {
        await _licenseService.DeleteAsync(licenseDto).ConfigureAwait(false);
        return Ok(licenseDto);
    }
}