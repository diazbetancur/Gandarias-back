using CC.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Domain.Dtos
{
    public class SigningDto : BaseDto<Guid>
    {
        public Guid UserId { get; set; }
        public string? UserFullName { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public SigningType TipoFichaje { get; set; }
        public string Observaciones { get; set; }
        public Guid IdUserLastUpdate { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}