using CC.Domain.Dtos;
using CC.Domain.Enums;
using CC.Domain.Interfaces.Services;
using CC.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace CC.Infrastructure.Repositories;

public class QrCodeRepository : IQrCodeService
{
    private readonly QrCodeOptions _options;
    private readonly IEncryptionService _encryptionService;
    //public QrCodeRepository(
    //    IUserService userService,
    //    IOptions<QrCodeOptions> options)
    //{
    //    _userService = userService;
    //    _options = options.Value;

    public QrCodeRepository(IEncryptionService encryptionService, IOptions<QrCodeOptions> options)
    {
        _encryptionService = encryptionService;
        _options = options.Value;
    }

    public async Task<(string encryptedToken, byte[] qrCodeBytes)> GenerateWeeklyTokenAsync(Guid userId, DateOnly weekStart)
    {
        var weekEnd = weekStart.AddDays(6);
        var validUntil = weekEnd.ToDateTime(TimeOnly.MaxValue).AddDays(1);

        var tokenData = new
        {
            UserId = userId,
            WeekStart = weekStart.ToString("yyyy-MM-dd"),
            WeekEnd = weekEnd.ToString("yyyy-MM-dd"),
            ValidUntil = validUntil.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };

        var jsonToken = JsonSerializer.Serialize(tokenData);
        var encryptedToken = await _encryptionService.EncryptAsync(jsonToken);

        CancellationToken cancellationToken = default;

        //var qrCodeBytes = await _qrCodeGenerator.GenerateQrCodeAsync(encryptedToken);
        var qrCodeBytes = await GenerateQrBytesAsync(encryptedToken, cancellationToken);

        return (encryptedToken, qrCodeBytes);
    }

    public async Task<QrTokenValidationResult> ValidateTokenAsync(string encryptedToken)
    {
        //try
        //{
        //    // Buscar token en base de datos
        //    var dbToken = await _tokenRepository.GetFirstOrDefaultAsync(
        //        x => x.EncryptedToken == encryptedToken && x.IsActive,
        //        includeProperties: "User"
        //    );

        //    if (dbToken == null)
        //    {
        //        return new QrTokenValidationResult
        //        {
        //            IsValid = false,
        //            ErrorMessage = "Token no encontrado o inactivo"
        //        };
        //    }

        //    // Verificar expiración
        //    if (dbToken.ValidUntil <= DateTime.UtcNow)
        //    {
        //        return new QrTokenValidationResult
        //        {
        //            IsValid = false,
        //            ErrorMessage = "Token expirado"
        //        };
        //    }

        //    // Descifrar y validar contenido
        //    var decryptedJson = await _encryptionService.DecryptAsync(encryptedToken);
        //    var tokenData = JsonSerializer.Deserialize<dynamic>(decryptedJson);

        //    return new QrTokenValidationResult
        //    {
        //        IsValid = true,
        //        UserId = dbToken.UserId,
        //        UserName = dbToken.User?.UserNickName,
        //        WeekStart = dbToken.WeekStart,
        //        WeekEnd = dbToken.WeekEnd,
        //        ValidUntil = dbToken.ValidUntil,
        //        TokenId = dbToken.TokenId
        //    };
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Error validando token: {Token}", encryptedToken.Substring(0, 10) + "...");
        //    return new QrTokenValidationResult
        //    {
        //        IsValid = false,
        //        ErrorMessage = "Error validando token"
        //    };
        //}

        return null;
    }

    //private readonly IUserService _userService;

    //public QrCodeRepository(
    //    IUserService userService,
    //    IOptions<QrCodeOptions> options)
    //{
    //    _userService = userService;
    //    _options = options.Value;
    //}

    //public async Task<byte[]> GenerateUserQrAsync(Guid userId, CancellationToken cancellationToken = default)
    //{
    //    return await GenerateUserDetailQrAsync(userId, false, cancellationToken);
    //}

    //public async Task<byte[]> GenerateUserDetailQrAsync(
    //    Guid userId,
    //    bool includeDetails = false,
    //    CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        var user = await _userService.FindByIdAsync(userId);
    //        if (user == null)
    //        {
    //            throw new ArgumentException($"Usuario {userId} no encontrado");
    //        }

    //        var qrData = PrepareQrDataAsync(user);

    //        return await GenerateQrBytesAsync(qrData, cancellationToken);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw;
    //    }
    //}

    //private string PrepareQrDataAsync(UserDto user)
    //{
    //    var qrData = new
    //    {
    //        UserId = user.Id,
    //        Email = user.Email,
    //        GeneratedAt = DateTime.UtcNow
    //    };

    //    var json = System.Text.Json.JsonSerializer.Serialize(qrData, new JsonSerializerOptions
    //    {
    //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //    });

    //    return json;
    //}

    private async Task<byte[]> GenerateQrBytesAsync(string data, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            // Usar la implementación de ImageSharp que ya definimos antes
            var matrix = qrCodeData.ModuleMatrix;
            var moduleCount = matrix.Count;
            var imageSize = moduleCount * _options.PixelsPerModule;

            using var image = new Image<Rgb24>(imageSize, imageSize);

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < imageSize; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);
                    var matrixY = y / _options.PixelsPerModule;

                    for (int x = 0; x < imageSize; x++)
                    {
                        var matrixX = x / _options.PixelsPerModule;
                        var isDark = matrix[matrixY][matrixX];

                        pixelRow[x] = isDark
                            ? new Rgb24(0, 0, 0)
                            : new Rgb24(255, 255, 255);
                    }
                }
            });

            using var ms = new MemoryStream();
            image.Save(ms, new PngEncoder());
            return ms.ToArray();
        }, cancellationToken);
    }
}