using CC.Application.Services;
using CC.Domain.Dtos;
using CC.Domain.Entities;
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
    private readonly ISigningService _signingService;

    public SigningController(IQrCodeService qrCodeService, IUserService userService, IScheduleService scheduleService, ISigningService signingService)
    {
        _qrCodeService = qrCodeService;
        _userService = userService;
        _scheduleService = scheduleService;
        _signingService = signingService;
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

            var userExist = await _userService.GetAllAsync(x => x.Id == result.UserId && !x.IsDelete && !x.IsActive).ConfigureAwait(false);

            if (!userExist.Any())
            {
                return BadRequest(new
                {
                    IsValid = false,
                    Confirm = false,
                    ErrorMessage = "Usuario inválido o información inválida."
                });
            }

            var (currentDate, currentTime) = GetCurrentSpainDateTime();
            var user = userExist.First();

            var openSigning = await CloseSigning(user, currentDate, currentTime);

            if (openSigning.HasOpened)
            {
                if (openSigning.HasError)
                {
                    return BadRequest(new
                    {
                        IsValid = false,
                        Confirm = false,
                        ErrorMessage = openSigning.ErrorMessage
                    });
                }
                return Ok(new
                {
                    ErrorMessage = openSigning.ErrorMessage,
                    IsValid = true,
                    Confirm = false,
                });
            }

            var schedule = await _scheduleService.GetAllAsync(x => x.UserId == result.UserId && x.Date == currentDate).ConfigureAwait(false);

            if (!schedule.Any())
            {
                return Ok(new
                {
                    ErrorMessage = "El usuario no tiene horario asignado para hoy. ¿Desea fichar de todos modos?",
                    IsValid = true,
                    Confirm = true
                });
            }

            var message = await OpenSigning(user, currentDate, schedule.ToList(), currentTime);

            return Ok(new
            {
                ErrorMessage = message,
                IsValid = true,
                Confirm = false,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                IsValid = false,
                Confirm = false,
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// PUT api/Signing
    /// </summary>
    /// <param name="string"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Put(string DNI)
    {
        try
        {
            var userRole = User.GetRoles();
            if (!userRole.Any(x => x == RoleType.Coordinator.ToString()))
            {
                return Unauthorized();
            }

            var userExist = await _userService.GetAllAsync(x => x.UserName == DNI && !x.IsDelete && x.IsActive).ConfigureAwait(false);

            if (!userExist.Any())
            {
                return BadRequest(new
                {
                    IsValid = false,
                    Confirm = false,
                    ErrorMessage = "Usuario inválido o información inválida."
                });
            }

            var user = userExist.First();
            var (currentDate, currentTime) = GetCurrentSpainDateTime();

            var openSigning = await CloseSigning(user, currentDate, currentTime);

            if (openSigning.HasOpened)
            {
                if (openSigning.HasError)
                {
                    return BadRequest(new
                    {
                        IsValid = false,
                        Confirm = false,
                        ErrorMessage = openSigning.ErrorMessage
                    });
                }
                return Ok(new
                {
                    ErrorMessage = openSigning.ErrorMessage,
                    IsValid = true,
                    Confirm = false,
                });
            }

            var schedule = await _scheduleService.GetAllAsync(x => x.UserId == user.Id && x.Date == currentDate).ConfigureAwait(false);

            if (!schedule.Any())
            {
                return Ok(new
                {
                    ErrorMessage = "El usuario no tiene horario asignado para hoy. ¿Desea fichar de todos modos?",
                    IsValid = true,
                    Confirm = true,
                });
            }

            var messageOpen = await OpenSigning(user, currentDate, schedule.ToList(), currentTime);

            return Ok(new
            {
                ErrorMessage = messageOpen,
                IsValid = true,
                Confirm = false
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                IsValid = false,
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// POST api/Signing/Confirm - Confirmar fichaje sin horario
    /// </summary>
    /// <param name="token"></param>
    /// <param name="confirm"></param>
    /// <param name="DNI"></param>
    /// <returns></returns>
    [HttpPost("Confirm")]
    public async Task<IActionResult> PostConfirm(string token = null, string DNI = null, bool confirm = false)
    {
        try
        {
            var userRole = User.GetRoles();
            if (!userRole.Any(x => x == RoleType.Coordinator.ToString()))
            {
                return Unauthorized();
            }

            if (!confirm)
            {
                return BadRequest(new
                {
                    IsValid = false,
                    ErrorMessage = "Operación cancelada por el usuario."
                });
            }

            UserDto user = null;

            // Validar por token (QR) o por DNI (Plan B)
            if (!string.IsNullOrEmpty(token))
            {
                var result = await _qrCodeService.ValidateTokenAsync(token);
                if (!result.IsValid)
                {
                    return BadRequest(result);
                }

                var userExist = await _userService.GetAllAsync(x => x.Id == result.UserId && !x.IsDelete && x.IsActive).ConfigureAwait(false);
                if (!userExist.Any())
                {
                    return BadRequest(new
                    {
                        IsValid = false,
                        ErrorMessage = "Usuario inválido."
                    });
                }
                user = userExist.First();
            }
            else if (!string.IsNullOrEmpty(DNI))
            {
                var userExist = await _userService.GetAllAsync(x => x.UserName == DNI && !x.IsDelete && x.IsActive).ConfigureAwait(false);
                if (!userExist.Any())
                {
                    return BadRequest(new
                    {
                        IsValid = false,
                        ErrorMessage = "Usuario inválido."
                    });
                }
                user = userExist.First();
            }
            else
            {
                return BadRequest(new
                {
                    IsValid = false,
                    ErrorMessage = "Se requiere token o DNI para la confirmación."
                });
            }

            var (currentDate, currentTime) = GetCurrentSpainDateTime();

            // Verificar que no haya un turno ya abierto
            var openSigning = await _signingService.GetAllAsync(x => x.UserId == user.Id && x.Date == currentDate && x.EndTime == null).ConfigureAwait(false);

            if (openSigning.Any())
            {
                return BadRequest(new
                {
                    IsValid = false,
                    ErrorMessage = "El usuario ya tiene un turno abierto."
                });
            }

            // Crear nuevo fichaje sin horario asignado
            var newSigning = new SigningDto
            {
                UserId = user.Id,
                Date = currentDate,
                StartTime = currentTime,
                TipoFichaje = SigningType.SinTurnoAsignado,
                Observaciones = "Fichaje confirmado sin horario asignado",
                LastUpdateUserId = null,
                UpdatedAt = DateTime.UtcNow
            };

            await _signingService.AddAsync(newSigning).ConfigureAwait(false);

            return Ok(new
            {
                ErrorMessage = "Turno iniciado correctamente sin horario asignado",
                IsValid = true,
                Confirm = false
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                IsValid = false,
                ErrorMessage = ex.Message
            });
        }
    }

    private async Task<SigningResponse> CloseSigning(UserDto userDto, DateOnly currentDate, TimeOnly time)
    {
        try
        {
            var openSigning = await _signingService.GetAllAsync(x => x.UserId == userDto.Id && x.Date == currentDate && x.EndTime == null).ConfigureAwait(false);

            if (!openSigning.Any())
            {
                return new SigningResponse
                {
                    ErrorMessage = "No hay turno abierto para cerrar.",
                    IsValid = false,
                    Confirm = false,
                    HasError = true,
                    HasOpened = false
                };
            }

            var signing = openSigning.First();
            signing.EndTime = time;
            signing.LastUpdateUserId = userDto.Id;
            signing.UpdatedAt = DateTime.Now;

            await _signingService.UpdateAsync(signing).ConfigureAwait(false);

            return new SigningResponse
            {
                ErrorMessage = "Turno finalizado correctamente",
                IsValid = true,
                Confirm = false,
                HasError = false,
                HasOpened = true
            };
        }
        catch
        {
            return new SigningResponse
            {
                ErrorMessage = "Error al cerrar el fichaje, por favor contacte al administrador.",
                IsValid = false,
                Confirm = false,
                HasError = true,
                HasOpened = true
            };
        }
    }

    private async Task<string> OpenSigning(UserDto userDto, DateOnly currentDate, List<ScheduleDto> schedules, TimeOnly time)
    {
        try
        {
            var blocks = GroupSchedulesIntoBlocks(schedules);
            var currentBlock = FindCurrentBlock(blocks, time);
            var signingType = GetSigningTypeForBlock(currentBlock, time);

            var newSigning = new SigningDto
            {
                UserId = userDto.Id,
                Date = currentDate,
                StartTime = time,
                TipoFichaje = signingType,
                Observaciones = string.Empty,
                LastUpdateUserId = userDto.Id,
                UpdatedAt = DateTime.Now
            };

            await _signingService.AddAsync(newSigning).ConfigureAwait(false);
            return "Turno iniciado correctamente";
        }
        catch
        {
            throw new Exception("Error al procesar el fichaje, por favor contacte al administrador.");
        }
    }

    private List<List<ScheduleDto>> GroupSchedulesIntoBlocks(List<ScheduleDto> schedules)
    {
        var ordered = schedules.OrderBy(s => s.StartTime).ToList();
        var blocks = new List<List<ScheduleDto>>();
        var currentBlock = new List<ScheduleDto>();

        foreach (var schedule in ordered)
        {
            if (!currentBlock.Any())
            {
                currentBlock.Add(schedule);
            }
            else
            {
                var last = currentBlock.Last();
                if (schedule.StartTime <= last.EndTime)
                {
                    currentBlock.Add(schedule);
                }
                else
                {
                    blocks.Add(currentBlock);
                    currentBlock = new List<ScheduleDto> { schedule };
                }
            }
        }

        if (currentBlock.Any())
            blocks.Add(currentBlock);

        return blocks;
    }

    private List<ScheduleDto>? FindCurrentBlock(List<List<ScheduleDto>> blocks, TimeOnly currentTime)
    {
        foreach (var block in blocks)
        {
            var start = block.First().StartTime;
            var end = block.Last().EndTime;

            if (start.HasValue && end.HasValue)
            {
                var startTimeOnly = TimeOnly.FromTimeSpan(start.Value);
                var endTimeOnly = TimeOnly.FromTimeSpan(end.Value);

                if (currentTime >= startTimeOnly && currentTime <= endTimeOnly)
                    return block;
            }
        }

        return null;
    }

    private SigningType GetSigningTypeForBlock(List<ScheduleDto> block, TimeOnly currentTime)
    {
        if (block == null || !block.Any())
            return SigningType.SinTurnoAsignado;

        var blockStart = block.First().StartTime;

        if (!blockStart.HasValue)
            return SigningType.SinTurnoAsignado;

        var blockStartTimeOnly = TimeOnly.FromTimeSpan(blockStart.Value);
        var diffMinutes = (currentTime - blockStartTimeOnly).TotalMinutes;

        if (diffMinutes < -60)
            return SigningType.IngresoAntesDeTurno;

        if (diffMinutes > 60)
            return SigningType.IngresoTarde;

        return SigningType.Normal;
    }

    private (DateOnly currentDate, TimeOnly currentTime) GetCurrentSpainDateTime()
    {
        var spainTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");
        var spainDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, spainTimeZone);
        var currentDate = new DateOnly(spainDateTime.Year, spainDateTime.Month, spainDateTime.Day);
        var currentTime = new TimeOnly(spainDateTime.Hour, spainDateTime.Minute);
        return (currentDate, currentTime);
    }
}