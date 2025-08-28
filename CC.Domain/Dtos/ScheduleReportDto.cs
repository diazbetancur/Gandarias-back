using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Domain.Dtos
{
    public class ScheduleReportDto
    {
        public string? WorkstationName { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? Entry { get; set; }
        public TimeOnly? Exit { get; set; }
        public double? TotalHours { get; set; }
    }
}