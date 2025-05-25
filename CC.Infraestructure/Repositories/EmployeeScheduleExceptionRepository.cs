using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class EmployeeScheduleExceptionRepository : ERepositoryBase<EmployeeScheduleException>, IEmployeeScheduleExceptionRepository
{
    public EmployeeScheduleExceptionRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}