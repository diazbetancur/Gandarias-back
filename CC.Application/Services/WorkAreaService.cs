using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;

namespace CC.Application.Services;

public class WorkAreaService : ServiceBase<WorkArea, WorkAreaDto>, IWorkAreaService
{
    private readonly IWorkAreaRepository _repository;

    public WorkAreaService(IWorkAreaRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _repository = repository;
    }

    public override Task<bool> DeleteAsync(WorkAreaDto entity)
    {
        return _repository.DeleteAsync(entity);
    }
}