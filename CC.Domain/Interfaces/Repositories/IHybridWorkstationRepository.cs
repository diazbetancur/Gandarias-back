using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Repositories;

public interface IHybridWorkstationRepository : IERepositoryBase<HybridWorkstation>
{
    Task<HybridWorkstation> AddAsync(HybridWorkstationDto entity);
}