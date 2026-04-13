namespace ocrProje.Models;

// ── API'den dönecek tam yanıt modeli ─────────────────────────────────────────
public class ExtractionResponse
{
    // Kullanıcı girişi & intent çözümleme
    public string UserQuery        { get; set; } = "";
    /// <summary>Standart alan anahtarı (örn. ALICI_IBAN, HASTA_TCKN).</summary>
    public string CanonicalField   { get; set; } = "";
    /// <summary>Stop-word temizlenmiş kullanıcı ifadesi; vision prompt ile uyumludur.</summary>
    public string UserQueryClean   { get; set; } = "";
    /// <summary>Eski istemciler için: genelde <see cref="UserQueryClean"/> ile aynı.</summary>
    public string ResolvedField    { get; set; } = "";
    public string IntentMethod     { get; set; } = "";
    /// <summary>Çoklu çıpa adayında kısa açıklama; tek adayda null olabilir.</summary>
    public string? AnchorMatchNote { get; set; }

    // Temel alanlar
    /// <summary>Çıkarılan mantıksal alan adı (canonical).</summary>
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
