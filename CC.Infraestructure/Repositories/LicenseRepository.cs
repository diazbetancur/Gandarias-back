using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Infrastructure.Configurations;

namespace CC.Infrastructure.Repositories;

public class LicenseRepository : ERepositoryBase<License>, ILicenseRepository
{
    private readonly DBContext _dataContext;

    public LicenseRepository(IQueryableUnitOfWork unitOfWork, DBContext dBContext) : base(unitOfWork)
    {
        _dataContext = dBContext;
    }

    public async Task<License> CreateAsync(License license)
    {
        var add = await _dataContext.Licenses.AddAsync(license).ConfigureAwait(false);
        _dataContext.Commit();
        return add.Entity;
    }
}