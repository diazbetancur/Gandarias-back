using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CC.Infrastructure.Repositories;

public class UserRestrictionShiftRepository : ERepositoryBase<UserRestrictionShift>, IUserRestrictionShiftRepository
{
    private readonly DBContext _dataContext;

    public UserRestrictionShiftRepository(IQueryableUnitOfWork unitOfWork, DBContext dB) : base(unitOfWork)
    {
        _dataContext = dB;
    }

    public async Task<UserRestrictionShift> AddAsync(UserRestrictionShiftDto entity)
    {
        try
        {
            var data = new UserRestrictionShift
            {
                IsDelete = false,
                IsRestricted = entity.IsRestricted,
                Observation = entity.Observation,
                ShiftTypeId = entity.ShiftTypeId,
                UserId = entity.UserId,
            };
            _dataContext.Add(data);

            await _dataContext.SaveChangesAsync().ConfigureAwait(false);

            return data;
        }
        catch (Exception)
        {
            throw;
        }
    }
}