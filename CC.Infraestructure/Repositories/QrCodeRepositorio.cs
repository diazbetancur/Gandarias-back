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

namespace CC.Infrastructure.Repositories;

public class QrCodeRepository : IQrCodeService
{
    private readonly IUserService _userService;
    private readonly QrCodeOptions _options;

    public QrCodeRepository(
        IUserService userService,
        IOptions<QrCodeOptions> options)
    {
        _userService = userService;
        _options = options.Value;
    }

    public async Task<byte[]> GenerateUserQrAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await GenerateUserDetailQrAsync(userId, false, cancellationToken);
    }

    public async Task<byte[]> GenerateUserDetailQrAsync(
        Guid userId,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"Usuario {userId} no encontrado");
            }

            var qrData = PrepareQrDataAsync(user);

            return await GenerateQrBytesAsync(qrData, cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private string PrepareQrDataAsync(UserDto user)
    {
        var qrData = new
        {
            UserId = user.Id,
            Email = user.Email,
            GeneratedAt = DateTime.UtcNow
        };

        var json = System.Text.Json.JsonSerializer.Serialize(qrData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return json;
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