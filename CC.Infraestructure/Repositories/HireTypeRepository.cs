

using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class HireTypeRepository : ERepositoryBase<HireType>, IHireTypeRepository
{
    public HireTypeRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}
