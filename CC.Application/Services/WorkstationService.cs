using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;

namespace CC.Application.Services;

public class WorkstationService : ServiceBase<Workstation, WorkstationDto>, IWorkstationService
{
    public WorkstationService(IWorkstationRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}