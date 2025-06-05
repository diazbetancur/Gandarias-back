using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class WorkstationDemandRepository : ERepositoryBase<WorkstationDemand>, IWorkstationDemandRepository
{
    public WorkstationDemandRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}