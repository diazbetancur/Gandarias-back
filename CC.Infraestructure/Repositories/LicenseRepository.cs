using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories;

public class LicenseRepository : ERepositoryBase<License>, ILicenseRepository
{
    public LicenseRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}