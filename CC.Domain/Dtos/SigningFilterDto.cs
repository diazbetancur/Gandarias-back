using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Domain.Dtos
{
    public class SigningFilterDto
    {
        public Guid? UserId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }
}