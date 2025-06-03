using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class LawRestrictionRepository : ERepositoryBase<LawRestriction>, ILawRestrictionRepository
{
    public LawRestrictionRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}