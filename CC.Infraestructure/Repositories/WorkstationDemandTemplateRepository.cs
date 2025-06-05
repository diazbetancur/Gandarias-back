using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class WorkstationDemandTemplateRepository : ERepositoryBase<WorkstationDemandTemplate>, IWorkstationDemandTemplateRepository
{
    public WorkstationDemandTemplateRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}