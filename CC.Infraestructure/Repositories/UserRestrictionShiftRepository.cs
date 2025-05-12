using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Infrastructure.Repositories;

public class UserRestrictionShiftRepository : ERepositoryBase<UserRestrictionShift>, IUserRestrictionShiftRepository
{
    public UserRestrictionShiftRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}