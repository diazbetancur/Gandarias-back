using CC.Domain.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace CC.Domain.Interfaces.Services;

public interface IQrCodeService
{
    Task<(string encryptedToken, byte[] qrCodeBytes)> GenerateWeeklyTokenAsync(Guid userId, DateOnly weekStart);

    Task<QrTokenValidationResult> ValidateTokenAsync(string encryptedToken);
}