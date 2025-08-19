using CC.Application.Services;
using CC.Domain.Dtos;
using CC.Domain.Enums;
using CC.Domain.Helpers;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SigningController : ControllerBase
{
    private readonly IQrCodeService _qrCodeService;
    private readonly IUserService _userService;
    private readonly IScheduleService _scheduleService;

    public SigningController(IQrCodeService qrCodeService, IUserService userService, IScheduleService scheduleService)
    {
        _qrCodeService = qrCodeService;
        _userService = userService;
        _scheduleService = scheduleService;
    }

    /// <summary>
    /// POST api/Signing
    /// </summary>
    /// <param name="string"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(string token)
    {
        try
        {
            var userRole = User.GetRoles();
            if (!userRole.Any(x => x == RoleType.Coordinator.ToString()))
            {
                return Unauthorized();
            }

            var result = await _qrCodeService.ValidateTokenAsync(token);

            if (!result.IsValid)
            {
                return BadRequest(result);
            }

            var userExist = await _userService.GetAllAsync(x => x.Id == result.UserId).ConfigureAwait(false);

            if (!userExist.Any())
            {
                return BadRequest(new
                {
                    IsValid = false,
                    ErrorMessage = "Usuario invalido."
                }
                );
            }
            var currenteDate = DateTime.UtcNow;
            DateOnly date = new DateOnly(currenteDate.Year, currenteDate.Month, currenteDate.Day);
            var schedule = await _scheduleService.GetAllAsync(x => x.UserId == result.UserId && x.Date == date).ConfigureAwait(false);

            if (schedule.Any())
            {
            }

            return Ok("");
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// POST api/Signing
    /// </summary>
    /// <param token="string"></param>
    /// <param confirm="Boolean"></param>
    /// <returns></returns>
    [HttpPost("Confirm")]
    public async Task<IActionResult> PostConfirm(string token, Boolean confirm)
    {
        return (Ok());
    }
}