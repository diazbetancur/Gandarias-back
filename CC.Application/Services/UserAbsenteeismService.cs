using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;

namespace CC.Application.Services;

public class UserAbsenteeismService : ServiceBase<UserAbsenteeism, UserAbsenteeismDto>, IUserAbsenteeismService
{
    public UserAbsenteeismService(IUserAbsenteeismRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}