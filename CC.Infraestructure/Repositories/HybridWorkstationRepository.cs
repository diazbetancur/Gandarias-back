using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Infrastructure.Repositories;

public class HybridWorkstationRepository : ERepositoryBase<HybridWorkstation>, IHybridWorkstationRepository
{
    public HybridWorkstationRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}