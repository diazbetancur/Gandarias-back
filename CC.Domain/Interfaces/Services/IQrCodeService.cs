using CC.Domain.Dtos;

namespace CC.Domain.Interfaces.Services;

public interface IQrCodeService
{
    Task<byte[]> GenerateUserQrAsync(Guid userId, CancellationToken cancellationToken = default);
}