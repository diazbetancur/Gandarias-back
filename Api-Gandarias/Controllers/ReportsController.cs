using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;
using System.Linq;

namespace Gandarias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportsController : ControllerBase
    {
        private readonly ISigningService _signingService;
        private readonly IScheduleService _scheduleService;

        public ReportsController(ISigningService signingService, IScheduleService scheduleService)
        {
            _signingService = signingService;
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// GET api/Reports
        /// </summary>
        /// <returns></returns>
        [HttpGet("SigningReport")]
        public async Task<IActionResult> GetAllSigningAsync([FromQuery] SigningFilterDto filter)
        {
            var signings = await _signingService.GetAllAsync(x =>
                (!filter.UserId.HasValue || x.UserId == filter.UserId.Value) &&
                (!filter.StartDate.HasValue || x.Date >= filter.StartDate.Value) &&
                (!filter.EndDate.HasValue || x.Date <= filter.EndDate.Value), includeProperties: "User"
            ).ConfigureAwait(false);

            var report = signings
                .GroupBy(x => new { x.UserId, x.UserFullName, x.Date })
                .Select(g => new SigningReportDto
                {
                    UserId = g.Key.UserId,
                    UserFullName = g.Key.UserFullName,
                    Date = g.Key.Date,

                    // Calcular total directamente con LINQ
                    TotalHours = g
                        .Where(e => e.EndTime.HasValue)
                        .Sum(e => (e.EndTime.Value - e.StartTime).TotalHours),

                    // Contar total de registros para el día
                    TotalEntries = g.Count(),
                    Entry1 = g.OrderBy(x => x.StartTime).First().StartTime,
                    Exit1 = g.OrderBy(x => x.StartTime).First().EndTime,
                    Entry2 = g.Where(x => x.EndTime.HasValue)
                               .OrderByDescending(x => x.EndTime)
                               .FirstOrDefault()?.StartTime == g.OrderBy(x => x.StartTime).First().StartTime ? null : g.Where(x => x.EndTime.HasValue)
                               .OrderByDescending(x => x.EndTime)
                               .FirstOrDefault()?.StartTime,
                    Exit2 = g.Where(x => x.EndTime.HasValue)
                               .OrderByDescending(x => x.EndTime)
                               .FirstOrDefault()?.EndTime == g.OrderBy(x => x.StartTime).First().EndTime ? null : g.Where(x => x.EndTime.HasValue)
                               .OrderByDescending(x => x.EndTime)
                               .FirstOrDefault()?.EndTime,

                    // Primer y último registro
                    FirstEntry = g.OrderBy(x => x.StartTime).First().StartTime,
                    LastExit = g.Where(x => x.EndTime.HasValue)
                               .OrderByDescending(x => x.EndTime)
                               .FirstOrDefault()?.EndTime
                })
                .OrderBy(r => r.UserFullName)
                .ToList();

            //.Skip((filter.PageNumber - 1) * filter.PageSize)
            //.Take(filter.PageSize)
            //.ToList();

            return Ok(report);
            //return Ok(new PagedResult<SigningReportDto>
            //{
            //    Items = report,
            //    TotalCount = signings.Count(),
            //    PageNumber = filter.PageNumber,
            //    PageSize = filter.PageSize
            //});
        }

        /// <summary>
        /// GET api/Reports
        /// </summary>
        /// <returns></returns>
        [HttpGet("DetailAsyncByUser")]
        public async Task<IActionResult> GetDetailAsyncByUser(Guid userId, DateOnly date)
        {
            var schedules = await _scheduleService.GetAllAsync(
                x => x.UserId == userId && x.Date == date,
                includeProperties: "User,Workstation"
            ).ConfigureAwait(false);

            var reportList = schedules.Select(item => new ScheduleReportDto
            {
                WorkstationName = item.WorkstationName,
                Date = item.Date,
                Entry = item.StartTime.HasValue ? TimeOnly.FromTimeSpan(item.StartTime.Value) : null,
                Exit = item.EndTime.HasValue ? TimeOnly.FromTimeSpan(item.EndTime.Value) : null,
                TotalHours = CalculateWorkingHours(item.StartTime, item.EndTime)
            }).ToList();

            return Ok(reportList);
        }

        /// <summary>
        /// GET api/Reports
        /// </summary>
        /// <returns></returns>
        [HttpGet("ScheduleReport")]
        public async Task<IActionResult> GetAllScheduleAsync([FromQuery] SigningFilterDto filter)
        {
            var schedule = await _scheduleService.GetAllAsync(x =>
                (!filter.StartDate.HasValue || x.Date >= filter.StartDate.Value) &&
                (!filter.EndDate.HasValue || x.Date <= filter.EndDate.Value), includeProperties: "Workstation"
            ).ConfigureAwait(false);

            var report = schedule
                .GroupBy(x => new { x.WorkstationName, x.Date })
                .Select(g => new
                {
                    WorkstationName = g.Key.WorkstationName,
                    Date = g.Key.Date,
                    TotalHours = g.Sum(s =>
                    {
                        if (!s.StartTime.HasValue || !s.EndTime.HasValue)
                            return 0;

                        var duration = s.EndTime.Value - s.StartTime.Value;

                        if (duration.TotalHours < 0)
                        {
                            duration = duration.Add(TimeSpan.FromDays(1));
                        }

                        return duration.TotalHours;
                    })
                })
                .Where(x => x.WorkstationName != null)
                .OrderBy(r => r.WorkstationName)
                .ThenBy(r => r.Date)
                .ToList();

            return Ok(report);
        }

        private double? CalculateWorkingHours(TimeSpan? startTime, TimeSpan? endTime)
        {
            if (!startTime.HasValue || !endTime.HasValue)
                return null;

            var duration = endTime.Value - startTime.Value;

            if (duration.TotalHours < 0)
            {
                duration = duration.Add(TimeSpan.FromDays(1));
            }

            return Math.Round(duration.TotalHours, 2);
        }
    }
}