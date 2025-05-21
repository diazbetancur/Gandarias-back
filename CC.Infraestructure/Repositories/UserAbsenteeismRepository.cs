using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class UserAbsenteeismRepository : ERepositoryBase<UserAbsenteeism>, IUserAbsenteeismRepository
{
    public UserAbsenteeismRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}