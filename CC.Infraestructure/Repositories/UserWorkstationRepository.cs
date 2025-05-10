using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class UserWorkstationRepository : ERepositoryBase<UserWorkstation>, IUserWorkstationRepository
{
    public UserWorkstationRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}