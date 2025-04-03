using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Infrastructure.Configurations;

namespace CC.Infrastructure.Repositories;

public class WorkstationRepository : ERepositoryBase<Workstation>, IWorkstationRepository
{
    private readonly DBContext _dataContext;

    public WorkstationRepository(IQueryableUnitOfWork unitOfWork, DBContext dBContext) : base(unitOfWork)
    {
        _dataContext = dBContext;
    }

    public async Task<Workstation> AddAsync(Workstation entity)
    {
        var add = await _dataContext.Workstations.AddAsync(entity).ConfigureAwait(false);
        _dataContext.Commit();
        return add.Entity;
    }
}