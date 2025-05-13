using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Repositories;

public interface IUserRestrictionShiftRepository : IERepositoryBase<UserRestrictionShift>
{
    Task<UserRestrictionShift> AddAsync(UserRestrictionShiftDto entity);
}