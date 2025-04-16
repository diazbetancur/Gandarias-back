using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class HireTypeController : ControllerBase
{
    private readonly IHireTypeService _hireTypeService;

    public HireTypeController(IHireTypeService hireTypeService)
    {
        _hireTypeService = hireTypeService;
    }

    /// <summary>
    /// GET api/HireType
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _hireTypeService.GetAllAsync().ConfigureAwait(false));
    }
}