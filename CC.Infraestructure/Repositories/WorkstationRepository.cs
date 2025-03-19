using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class WorkstationRepository : ERepositoryBase<Workstation>, IWorkstationRepository
{
    public WorkstationRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}