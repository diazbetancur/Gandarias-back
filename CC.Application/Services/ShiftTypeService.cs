using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;

namespace CC.Application.Services;

public class ShiftTypeService : ServiceBase<ShiftType, ShiftTypeDto>, IShiftTypeService
{
    public ShiftTypeService(IShiftTypeRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}