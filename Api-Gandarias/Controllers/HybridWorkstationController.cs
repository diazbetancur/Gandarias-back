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
public class HybridWorkstationController : ControllerBase
{
    private readonly IHybridWorkstationService _hybridWorkstationService;

    public HybridWorkstationController(IHybridWorkstationService hybridWorkstationService)
    {
        _hybridWorkstationService = hybridWorkstationService;
    }

    /// <summary>
    /// GET api/HybridWorkstation
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _hybridWorkstationService.GetAllAsync(x => !x.IsDeleted, includeProperties: "WorkstationA,WorkstationB,WorkstationC,WorkstationD").ConfigureAwait(false));
    }

    /// <summary>
    /// GET api/HybridWorkstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok(await _hybridWorkstationService.FindByIdAsync(id).ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/HybridWorkstation
    /// </summary>
    /// <param name="HybridWorkstationDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(HybridWorkstationDto hybridWorkstationDto)
    {
        await _hybridWorkstationService.AddAsync(hybridWorkstationDto).ConfigureAwait(false);
        return Ok(hybridWorkstationDto);
    }

    /// <summary>
    /// PUT api/HybridWorkstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="id"></param>
    /// <param name="HybridWorkstationDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, HybridWorkstationDto hybridWorkstationDto)
    {
        hybridWorkstationDto.Id = id;
        await _hybridWorkstationService.UpdateAsync(hybridWorkstationDto).ConfigureAwait(false);
        return Ok(hybridWorkstationDto);
    }

    /// <summary>
    /// DELETE api/HybridWorkstation/c5b257e0-e73f-4f34-a30c-c0e139ad8e58
    /// </summary>
    /// <param name="HybridWorkstationDto"></param>
    /// <returns></returns>
    [HttpDelete()]
    public async Task<IActionResult> Delete(HybridWorkstationDto hybridWorkstationDto)
    {
        await _hybridWorkstationService.DeleteAsync(hybridWorkstationDto).ConfigureAwait(false);
        return Ok(hybridWorkstationDto);
    }
}