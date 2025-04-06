using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Services;

public interface IWorkAreaService : IServiceBase<WorkArea, WorkAreaDto>
{
    Task<bool> DeleteAsync(WorkAreaDto entity);
}