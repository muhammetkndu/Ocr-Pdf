using ocrProje.Services;

namespace ocrProje.Models;

public class DocumentSession
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string OriginalFilePath { get; set; } = "";
    public string PreprocessedImagePath { get; set; } = "";
    public string DocType { get; set; } = "DIGER";
    
    // OCR tarafından 1 kez taranmış tüm satırlar ve koordinatları
    public List<AnchorCoordinate> ScannedLines { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
