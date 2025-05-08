using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Infrastructure.Repositories;

public class UserWorkstationRepository : ERepositoryBase<UserWorkstation>, IUserWorkstationRepository
{
    public UserWorkstationRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}