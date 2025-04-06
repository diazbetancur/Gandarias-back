using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Repositories;

public interface IWorkstationRepository : IERepositoryBase<Workstation>
{
    Task<bool> DeleteAsync(WorkstationDto entity);
}