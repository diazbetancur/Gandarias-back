using CC.Domain.Dtos;
using CC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Domain.Interfaces.Services;

public interface IUserWorkstationService : IServiceBase<UserWorkstation, UserWorkstationDto>
{
}