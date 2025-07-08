using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class EmployeeShiftRestrictionRepository : ERepositoryBase<EmployeeShiftTypeRestriction>, IEmployeeShiftRestrictionRepository
{
    public EmployeeShiftRestrictionRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}