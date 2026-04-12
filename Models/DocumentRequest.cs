namespace ocrProje.Models;

// Bu model gelecekte kullanılmak üzere ayrılmıştır.
// Şu an /api/extract endpoint'i [FromForm] ile doğrudan IFormFile ve string alıyor.
// Daha karmaşık request yapıları gerekirse buraya taşınabilir.

/// <summary>
/// Belge okuma isteği için model (henüz aktif kullanımda değil).
/// </summary>
public class DocumentRequest
{
    public string TargetField { get; set; } = string.Empty;
    public string? FileName   { get; set; }
}
