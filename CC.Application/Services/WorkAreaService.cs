using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;

namespace CC.Application.Services;

public class WorkAreaService : ServiceBase<WorkArea, WorkAreaDto>, IWorkAreaService
{
    public WorkAreaService(IWorkAreaRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}