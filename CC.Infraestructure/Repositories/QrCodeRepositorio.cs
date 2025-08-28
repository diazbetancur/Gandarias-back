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
        try
        {
            if (string.IsNullOrEmpty(encryptedToken) || string.IsNullOrWhiteSpace(encryptedToken))
            {
                return new QrTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token vacío o nulo"
                };
            }

            var decryptedJson = await _encryptionService.DecryptAsync(encryptedToken);
            using var jsonDoc = JsonDocument.Parse(decryptedJson);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("UserId", out var userIdElement) ||
                !root.TryGetProperty("ValidUntil", out var validUntilElement) ||
                !root.TryGetProperty("WeekStart", out var weekStartElement) ||
                !root.TryGetProperty("WeekEnd", out var weekEndElement))
            {
                return new QrTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token inválido"
                };
            }

            if (!Guid.TryParse(userIdElement.GetString(), out var userId) ||
                !DateTime.TryParse(validUntilElement.GetString(), out var validUntil) ||
                !DateOnly.TryParse(weekStartElement.GetString(), out var weekStart) ||
                !DateOnly.TryParse(weekEndElement.GetString(), out var weekEnd))
            {
                return new QrTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token inválido"
                };
            }

            if (validUntil <= DateTime.UtcNow)
            {
                return new QrTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token inválido",
                    UserId = userId,
                    WeekStart = weekStart,
                    WeekEnd = weekEnd,
                    ValidUntil = validUntil
                };
            }
            return new QrTokenValidationResult
            {
                IsValid = true,
                UserId = userId,
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                ValidUntil = validUntil
            };
        }
        catch (Exception)
        {
            return new QrTokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Error interno validando token"
            };
        }
    }

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