using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Domain.Dtos
{
    public class SigningReportDto
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? Entry1 { get; set; }
        public TimeOnly? Exit1 { get; set; }
        public TimeOnly? Entry2 { get; set; }
        public TimeOnly? Exit2 { get; set; }
        public double? TotalHours { get; set; }
    }
}