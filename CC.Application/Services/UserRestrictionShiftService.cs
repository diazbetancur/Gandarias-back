using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;
using CC.Infrastructure.Repositories;

namespace CC.Application.Services;

public class UserRestrictionShiftService : ServiceBase<UserRestrictionShift, UserRestrictionShiftDto>, IUserRestrictionShiftService
{
    public readonly IUserRestrictionShiftRepository _userRestrictionShiftRepository;

    public UserRestrictionShiftService(IUserRestrictionShiftRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _userRestrictionShiftRepository = repository;
    }

    public async Task<UserRestrictionShift> AddAsync(UserRestrictionShiftDto entity)
    {
        return await _userRestrictionShiftRepository.AddAsync(entity);
    }
}