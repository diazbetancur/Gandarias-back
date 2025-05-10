using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Repositories;

public class HybridWorkstationRepository : ERepositoryBase<HybridWorkstation>, IHybridWorkstationRepository
{
    private readonly DBContext _dataContext;

    public HybridWorkstationRepository(IQueryableUnitOfWork unitOfWork, DBContext dB) : base(unitOfWork)
    {
        _dataContext = dB;
    }

    public async Task<HybridWorkstation> AddAsync(HybridWorkstationDto entity)
    {
        try
        {
            var exist = await _dataContext.HybridWorkstations.FirstOrDefaultAsync(x => (x.WorkstationAId == entity.WorkstationAId && x.WorkstationBId == entity.WorkstationBId)
            || (x.WorkstationBId == entity.WorkstationAId && x.WorkstationAId == entity.WorkstationBId));
            if (exist != null)
            {
                throw new Exception("Ya existe la combinacion de puestos hibridos que desea crear.");
            }

            exist = new HybridWorkstation()
            {
                WorkstationBId = entity.WorkstationBId,
                WorkstationAId = entity.WorkstationAId,
                Id = new Guid(),
                Description = entity.Description,
                IsDeleted = false,
            };
            _dataContext.Add(exist);

            await _dataContext.SaveChangesAsync().ConfigureAwait(false);

            return exist;
        }
        catch (Exception)
        {
            throw;
        }
    }
}