using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;
using CC.Domain.Services;

namespace CC.Application.Services;

public class LicenseService : ServiceBase<License, LicenseDto>, ILicenseService
{
    private readonly ILicenseRepository _licenseRepository;

    public LicenseService(ILicenseRepository repository, IMapper mapper, ILicenseRepository licenseRepository) : base(repository, mapper)
    {
        _licenseRepository = licenseRepository;
    }

    public async Task<License> CreateAsync(LicenseDto license)
    {
        var entity = new License
        {
            Id = Guid.NewGuid(),
            EndDate = license.EndDate,
            DaysRequested = license.DaysRequested ?? 1,
            HalfPeriod = license.HalfPeriod,
            Observation = license.Observation,
            Reason = license.Reason,
            UserId = license.UserName,
            IsHalfDay = license.IsHalfDay,
            IsDeleted = false,
            StartDate = license.StartDate,
        };

        await _licenseRepository.AddAsync(entity).ConfigureAwait(false);

        return entity;
    }
}