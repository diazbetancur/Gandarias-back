using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;

namespace CC.Application.Services;

public class WorkstationService : ServiceBase<Workstation, WorkstationDto>, IWorkstationService
{
    private readonly IWorkstationRepository _repository;
    public WorkstationService(IWorkstationRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _repository = repository;
    }

    public override async Task<WorkstationDto> AddAsync(WorkstationDto entityDto)
    {
        var entity = new Workstation
        {
            Name = entityDto.Name,
            WorkAreaId = entityDto.WorkAreaId,
            IsActive = true,
            Id = Guid.NewGuid()
        };

        await _repository.AddAsync(entity).ConfigureAwait(false);

        return entityDto;
    }
}