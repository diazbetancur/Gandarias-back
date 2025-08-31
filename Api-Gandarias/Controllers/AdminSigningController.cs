using CC.Domain.Dtos;
using CC.Domain.Helpers;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gandarias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdminSigningController : ControllerBase
    {
        private readonly ISigningService _signingService;

        public AdminSigningController(ISigningService signingService)
        {
            _signingService = signingService;
        }

        /// <summary>
        /// GET api/AdminSigning
        /// </summary>
        /// <param name="SigningFilterDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] SigningFilterDto filter)
        {
            var signings = await _signingService.GetAllAsync(x =>
                (!filter.UserId.HasValue || x.UserId == filter.UserId.Value) &&
                (!filter.StartDate.HasValue || x.Date >= filter.StartDate.Value) &&
                (!filter.EndDate.HasValue || x.Date <= filter.EndDate.Value), includeProperties: "User"
            ).ConfigureAwait(false);

            return Ok(signings);
        }

        /// <summary>
        /// POST api/AdminSigning
        /// </summary>
        /// <param name="SigningDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(SigningDto signingDto)
        {
            var result = await _signingService.AddAsync(signingDto).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// PUT api/AdminSigning
        /// </summary>
        /// <param name="SigningDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put(SigningDto signingDto)
        {
            var existingSigning = await _signingService.GetAllAsync(x => x.Id == signingDto.Id).ConfigureAwait(false);
            if (!existingSigning.Any())
            {
                return BadRequest("Fichaje no encontrado");
            }
            await _signingService.UpdateAsync(signingDto).ConfigureAwait(false);
            return Ok(signingDto);
        }

        /// <summary>
        /// DELETE api/AdminSigning
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingSigning = await _signingService.GetAllAsync(x => x.Id == id).ConfigureAwait(false);
            if (!existingSigning.Any())
            {
                return BadRequest("Fichaje no encontrado");
            }
            await _signingService.DeleteAsync(existingSigning.First()).ConfigureAwait(false);
            return Ok(existingSigning);
        }
    }
}