using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Services;

public interface ILicenseService : IServiceBase<License, LicenseDto>
{
    Task<License> CreateAsync(LicenseDto license);
}