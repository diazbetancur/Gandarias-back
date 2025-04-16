using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class ShiftTypeRepository : ERepositoryBase<ShiftType>, IShiftTypeRepository
{
    public ShiftTypeRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}