namespace CC.Domain.Dtos;

public class QrCodeResponseDto
{
    public string Base64Data { get; set; } = string.Empty;
    public byte[] ImageBytes { get; set; } = Array.Empty<byte>();
    public string MimeType { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
}