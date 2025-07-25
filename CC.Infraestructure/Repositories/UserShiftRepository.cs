using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class UserShiftRepository : ERepositoryBase<UserShift>, IUserShiftRepository
{
    public UserShiftRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}