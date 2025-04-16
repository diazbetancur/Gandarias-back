using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Repositories;

public class WorkAreaRepository : ERepositoryBase<WorkArea>, IWorkAreaRepository
{
    private readonly DBContext _dataContext;

    public WorkAreaRepository(IQueryableUnitOfWork unitOfWork, DBContext dBContext) : base(unitOfWork)
    {
        _dataContext = dBContext;
    }

    public async Task<bool> DeleteAsync(WorkAreaDto entity)
    {
        try
        {
            var workArea = await _dataContext.WorkAreas.Include(x => x.workstations).FirstOrDefaultAsync(x => x.Id == entity.Id).ConfigureAwait(false);
            if (workArea == null)
            {
                return false;
            }

            workArea.IsDeleted = true;
            _dataContext.Update(workArea);

            foreach (var workstation in workArea.workstations)
            {
                workstation.IsDeleted = true;
                _dataContext.Update(workstation);
            }

            await _dataContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            throw new Exception("Error al eliminar WorkArea, por favor contactar al administrador.");
        }
    }
}