using CC.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace CC.Infrastructure.Repositories;

public class AesEncryptionService : IEncryptionService
{
    private readonly string _key;
    private readonly string _iv;

    public AesEncryptionService(IConfiguration configuration)
    {
        _key = configuration["Encryption:Key"] ?? throw new ArgumentException("Encryption key not configured");
        _iv = configuration["Encryption:IV"] ?? throw new ArgumentException("Encryption IV not configured");
    }

    public async Task<string> EncryptAsync(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(_key);
        aes.IV = Convert.FromBase64String(_iv);

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);

        await swEncrypt.WriteAsync(plainText);
        await swEncrypt.FlushAsync();
        csEncrypt.FlushFinalBlock();

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public async Task<string> DecryptAsync(string encryptedText)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(_key);
        aes.IV = Convert.FromBase64String(_iv);

        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText));
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return await srDecrypt.ReadToEndAsync();
    }
}