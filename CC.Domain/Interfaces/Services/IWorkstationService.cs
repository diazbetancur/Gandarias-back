using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Services;

public interface IWorkstationService : IServiceBase<Workstation, WorkstationDto>
{
    Task<bool> DeleteAsync(WorkstationDto entity);
}