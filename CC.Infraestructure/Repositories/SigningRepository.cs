using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;

namespace CC.Infrastructure.Repositories
{
    public class SigningRepository : ERepositoryBase<Signing>, ISigningRepository
    {
        public SigningRepository(IQueryableUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}