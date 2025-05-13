using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Services;

public interface IUserRestrictionShiftService : IServiceBase<UserRestrictionShift, UserRestrictionShiftDto>
{
    Task<UserRestrictionShift> AddAsync(UserRestrictionShiftDto entity);
}