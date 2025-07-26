using CC.Domain.Enums;

namespace CC.Domain.Dtos;

public class QrCodeRequestDto
{
    public string Text { get; set; } = string.Empty;
    public int PixelsPerModule { get; set; } = 20;
    public string ForegroundColor { get; set; } = "#000000";
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public QrImageFormat Format { get; set; } = QrImageFormat.Png;
    public bool IncludeDataUri { get; set; } = true;
}