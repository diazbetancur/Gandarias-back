using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class WorkAreaRepository : ERepositoryBase<WorkArea>, IWorkAreaRepository
{
    public WorkAreaRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}