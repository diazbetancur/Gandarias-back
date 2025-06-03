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
public class LawRestrictionController : ControllerBase
{
    private readonly ILawRestrictionService _lawRestrictionService;

    public LawRestrictionController(ILawRestrictionService lawRestrictionService)
    {
        _lawRestrictionService = lawRestrictionService;
    }

    /// <summary>
    /// GET api/LawRestriction
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _lawRestrictionService.GetAllAsync().ConfigureAwait(false));
    }

    /// <summary>
    /// POST api/LawRestriction
    /// </summary>
    /// <param name="LawRestrictionDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(List<LawRestrictionDto> lawRestrictionDto)
    {
        try
        {
            foreach (var item in lawRestrictionDto)
            {
                await _lawRestrictionService.UpdateAsync(item).ConfigureAwait(false);
            }
            return Ok(lawRestrictionDto);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}