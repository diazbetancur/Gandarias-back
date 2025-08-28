using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;

namespace CC.Application.Services
{
    public class SigningService : ServiceBase<Signing, SigningDto>, ISigningService
    {
        public SigningService(ISigningRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}