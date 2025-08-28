using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;
using System.Linq;

namespace Gandarias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetAllSigningAsync(SigningFilterDto filter)
        {
            var signings = await _signingService.GetAllAsync(x =>
                (!filter.UserId.HasValue || x.UserId == filter.UserId.Value) &&
                (!filter.StartDate.HasValue || x.Date >= filter.StartDate.Value) &&
                (!filter.EndDate.HasValue || x.Date <= filter.EndDate.Value), includeProperties: "User"
            ).ConfigureAwait(false);

            var report = signings
                  .GroupBy(x => new { x.UserId, x.UserFullName, x.Date })
                  .Select(g =>
                  {
                      var ordered = g.OrderBy(x => x.StartTime).ToList();

                      // solo tomamos máximo 2 pares válidos
                      var entry1 = ordered.ElementAtOrDefault(0);
                      var entry2 = ordered.ElementAtOrDefault(1);

                      var reportItem = new SigningReportDto
                      {
                          UserId = g.Key.UserId,
                          UserFullName = g.Key.UserFullName,
                          Date = g.Key.Date,
                          Entry1 = entry1?.StartTime,
                          Exit1 = entry1?.EndTime,
                          Entry2 = entry2?.StartTime,
                          Exit2 = entry2?.EndTime,
                          TotalHours = CalculateTotalHours(entry1, entry2)
                      };

                      return reportItem;
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
        public async Task<IActionResult> GetDetailAsyncByUser(SigningFilterDto filter)
        {
            var result = await _scheduleService.GetAllAsync(x =>
                (!filter.UserId.HasValue || x.UserId == filter.UserId.Value) &&
                (!filter.StartDate.HasValue || x.Date >= filter.StartDate.Value) &&
                (!filter.EndDate.HasValue || x.Date <= filter.EndDate.Value), includeProperties: "User,Workstation"
            ).ConfigureAwait(false);

            List<ScheduleReportDto> reportList = new List<ScheduleReportDto>();
            foreach (var item in result)
            {
                reportList.Add(new ScheduleReportDto
                {
                    WorkstationName = item.WorkstationName,
                    Date = item.Date,
                    Entry = item.StartTime.HasValue ? TimeOnly.FromTimeSpan(item.StartTime.Value) : null,
                    Exit = item.EndTime.HasValue ? TimeOnly.FromTimeSpan(item.EndTime.Value) : null,
                    TotalHours = item.StartTime.HasValue && item.EndTime.HasValue ? (item.EndTime.Value - item.StartTime.Value).TotalHours : null
                });
            }

            return Ok(reportList);
        }

        /// <summary>
        /// GET api/Reports
        /// </summary>
        /// <returns></returns>
        [HttpGet("ScheduleReport")]
        public async Task<IActionResult> GetAllScheduleAsync(SigningFilterDto filter)
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
                        s.StartTime.HasValue && s.EndTime.HasValue
                            ? (s.EndTime.Value - s.StartTime.Value).TotalHours
                            : 0
                    )
                })
                .OrderBy(r => r.WorkstationName)
                .ThenBy(r => r.Date)
                .ToList();

            return Ok(report);
        }

        private double? CalculateTotalHours(dynamic entry1, dynamic entry2)
        {
            double total = 0;

            if (entry1?.EndTime != null)
                total += (entry1.EndTime.Value - entry1.StartTime).TotalHours;

            if (entry2?.StartTime != null && entry2?.EndTime != null)
                total += (entry2.EndTime.Value - entry2.StartTime.Value).TotalHours;

            return total > 0 ? total : null;
        }
    }
}