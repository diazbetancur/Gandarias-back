using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class AbsenteeismTypeRepository : ERepositoryBase<AbsenteeismType>, IAbsenteeismTypeRepository
{
    public AbsenteeismTypeRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}