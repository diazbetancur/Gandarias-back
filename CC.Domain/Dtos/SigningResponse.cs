using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Domain.Dtos
{
    public class SigningResponse
    {
        public bool IsValid { get; set; }
        public bool Confirm { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public bool HasOpened { get; set; }
    }
}