using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> DeleteAsync(WorkstationDto entity)
    {
        var workstation = await _dataContext.Workstations.FirstOrDefaultAsync(x => x.Id == entity.Id);
        if (workstation == null)
        {
            return false;
        }

        workstation.IsDeleted = true;
        _dataContext.Update(workstation);
        await _dataContext.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }
}