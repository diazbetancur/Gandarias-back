using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Repositories;

public interface IWorkAreaRepository : IERepositoryBase<WorkArea>
{
    Task<bool> DeleteAsync(WorkAreaDto entity);
}