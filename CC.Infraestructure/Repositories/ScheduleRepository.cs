using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class ScheduleRepository : ERepositoryBase<Schedule>, IScheduleRepository
{
    public ScheduleRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}