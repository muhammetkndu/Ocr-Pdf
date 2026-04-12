namespace ocrProje.Models;

// ── API'den dönecek tam yanıt modeli ─────────────────────────────────────────
public class ExtractionResponse
{
    // Kullanıcı girişi & intent çözümleme
    public string UserQuery        { get; set; } = "";   // "bana ibanı ver misin"
    public string ResolvedField    { get; set; } = "";   // "IBAN"  (OCR'da aranacak)
    public string IntentMethod     { get; set; } = "";   // "local" | "llm"

    // Temel alanlar
    public string FieldName            { get; set; } = "";
    public string ExtractedValue       { get; set; } = "";   // Gemini'nin döndürdüğü ham JSON
    public string CroppedImageBase64   { get; set; } = "";

    // Token analizi
    public int InputTokens             { get; set; }
    public int OutputTokens            { get; set; }
    public int TotalTokens             { get; set; }
    public double EstimatedCost        { get; set; }


    // Benzerlik analizi (OCR satırı vs LLM çıktısı)
    public string OcrLineText          { get; set; } = "";
    public string LlmExtractedText     { get; set; } = "";
    public double SimilarityScore      { get; set; }   // 0.0 → 1.0
    public bool   IsMatch              { get; set; }   // score >= 0.85
}

// ── OCR yardımcı model ────────────────────────────────────────────────────────
public record WordCoordinate(int X, int Y, int Width, int Height);