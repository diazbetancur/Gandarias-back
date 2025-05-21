using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class EmployeeScheduleRestrictionRepository : ERepositoryBase<EmployeeScheduleRestriction>, IEmployeeScheduleRestrictionRepository
{
    public EmployeeScheduleRestrictionRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}