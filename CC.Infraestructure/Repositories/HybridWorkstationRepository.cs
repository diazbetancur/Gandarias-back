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
            var incomingIds = new[] {
            entity.WorkstationAId,
            entity.WorkstationBId,
            entity.WorkstationCId,
            entity.WorkstationDId
        }.Where(x => x != null).Distinct().OrderBy(x => x).ToList();

            var allHybridWorkstations = await _dataContext.HybridWorkstations
    .AsNoTracking()
    .ToListAsync();

            var existing = allHybridWorkstations.FirstOrDefault(hw =>
            {
                var existingIds = new[] {
                hw.WorkstationAId,
                hw.WorkstationBId,
                hw.WorkstationCId,
                hw.WorkstationDId
            }.Where(x => x != null).Distinct().OrderBy(x => x).ToList();

                return incomingIds.SequenceEqual(existingIds);
            });

            if (existing == null)
            {
                var newEntity = new HybridWorkstation
                {
                    Id = Guid.NewGuid(),
                    WorkstationAId = entity.WorkstationAId,
                    WorkstationBId = entity.WorkstationBId,
                    WorkstationCId = entity.WorkstationCId,
                    WorkstationDId = entity.WorkstationDId,
                    Description = entity.Description,
                    IsDeleted = false,
                };

                _dataContext.Add(newEntity);
                await _dataContext.SaveChangesAsync();

                return newEntity;
            }

            if (existing.IsDeleted)
            {
                var reactivated = await _dataContext.HybridWorkstations
                    .FirstAsync(hw => hw.Id == existing.Id);

                reactivated.IsDeleted = false;
                _dataContext.Update(reactivated);
                await _dataContext.SaveChangesAsync();

                return reactivated;
            }

            throw new Exception("Ya existe la combinación de puestos híbridos que desea crear.");
        }
        catch (Exception)
        {
            throw;
        }
    }
}