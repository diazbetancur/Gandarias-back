﻿using CC.Domain.Entities;

namespace CC.Domain.Interfaces.Repositories;

public interface ILicenseRepository : IERepositoryBase<License>
{
    Task<License> CreateAsync(License license);
}