using System.Text.RegularExpressions;
using Tesseract;

namespace ocrProje.Services;

/// <summary>
/// Kullanıcının serbest metin sorgusunu (örn: "bana ibanı ver", "tc no") 
/// belgede aranacak KESİN STANDART ALAN ADINA (Canonical Key, örn: "IBAN", "TCKN") çevirir.
/// 
/// Strateji: 
/// 1. Önce yerel synonym haritası (hızlı, ücretsiz). Canonical Key bulursa direkt döner.
/// 2. Bulamazsa LLM Fallback: Gemini'ye mevcut Canonical Key listesi verilerek eşleştirme istenir.
/// </summary>
public class IntentResolverService
{
    private readonly GeminiService _gemini;

    // ── Yerel Synonym (Eş Anlamlılar) Haritası ────────────────────────────────
    // Her Canonical Key → kullanıcının yazabileceği ve PDF'te bulunabilecek tüm varyantlar
    private static readonly Dictionary<string, string[]> _synonymMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["IBAN"]       = ["iban", "hesap no", "banka hesabı", "hesap numarası", "banka numarası", "uluslararası hesap", "tr hesap"],
        ["TCKN"]       = ["tc", "tckn", "kimlik no", "kimlik numarası", "tc kimlik", "t.c. kimlik no", "tc no", "identity"],
        ["VKN"]        = ["vergi no", "vkn", "vergi kimlik", "vergi numarası", "vergi kimlik no", "tax"],
        ["AD_SOYAD"]   = ["isim", "ad soyad", "ad", "soyad", "müşteri adı", "hasta adı", "kişi", "adı soyadı", "name", "müşteri"],
        ["TUTAR"]      = ["tutar", "miktar", "fiyat", "ücret", "toplam", "para", "bedel", "amount", "total"],
        ["TARIH"]      = ["tarih", "işlem tarihi", "ne zaman", "düzenlenme", "kesilme tarihi", "date"],
        ["FATURA_NO"]  = ["fatura", "fatura no", "fatura numarası", "invoice", "invoice no", "fatura kodu"],
        ["VERGI_DAIRESI"] = ["vergi dairesi", "vd", "vergi ofisi"],
        ["SORGU_NO"]   = ["sorgu", "sorgu no", "referans", "ref no", "referans no", "reference"],
        ["KDV"]        = ["kdv", "katma değer", "vergi", "tax", "vat"],
        ["ALICI"]      = ["alıcı", "gönderilen", "hedef", "recipient", "alıcı adı"],
        ["GONDEREN"]   = ["gönderen", "gönderici", "sender", "kimden"],
        ["ACIKLAMA"]   = ["açıklama", "not", "description", "detay", "explanation"],
    };

    // Stop kelimeler — sorgudan temizlenir, sadece anahtar kelimeler kalır
    private static readonly HashSet<string> _stopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "bana", "ver", "göster", "istiyorum", "mısın", "misin", "musun", "musunuz",
        "sadece", "yalnızca", "acaba", "ne", "nedir", "nerede", "bir", "bu", "şu",
        "lütfen", "rica", "etsem", "edebilir", "misiniz", "misin", "verir",
        "verebilir", "öğrenebilir", "öğrenmek", "bilmek", "bilmek", "lazım",
        "gerekiyor", "gerekli", "almak", "almak", "için", "olan", "var", "yok",
        "the", "a", "an", "please", "show", "me", "give", "what", "is", "are"
    };

    public IntentResolverService(GeminiService gemini)
    {
        _gemini = gemini;
    }

    /// <summary>
    /// Verilen canonical alan adı için (örn: "TCKN") bilinen tüm eşanlamlıları döner.
    /// OCR'da etiket ararken (Label Matching) kullanılır.
    /// </summary>
    public static string[] GetSynonyms(string canonicalField)
    {
        if (_synonymMap.TryGetValue(canonicalField, out var syns))
            return syns;
        
        return new[] { canonicalField }; // Bilinmiyorsa kendini dön
    }

    /// <summary>
    /// Kullanıcının sorgusunu durma kelimelerinden temizleyip saf halini döndürür.
    /// (Örn: "bana hesap no ver" -> "hesap no")
    /// LLM'e prompt atarken IBAN değil kullanıcının kendi ifadesini geçirmek için kullanılır.
    /// </summary>
    public string GetCleanQuery(string query)
    {
        var words = Tokenize(query);
        return string.Join(" ", words);
    }

    /// <summary>
    /// Kullanıcının serbest metin sorgusunu sistemde tanımlı 'Canonical Key'lerden birine eşler.
    /// </summary>
    public async Task<(string ResolvedField, string Method)> ResolveAsync(string userQuery)
    {
        if (string.IsNullOrWhiteSpace(userQuery))
            throw new ArgumentException("Sorgu boş olamaz.");

        // 1. LOCAL MAP KONTROLÜ (En Güvenli ve En Hızlı)
        // Sorguyu temizle ve eşanlamlılar listesinde ara.
        var localCandidate = TryLocalMapping(userQuery);
        if (localCandidate != null)
        {
            Console.WriteLine($"[INTENT-LOCAL] '{userQuery}' -> Canonical Key: '{localCandidate}'");
            return (localCandidate, "local");
        }

        // 2. LLM FALLBACK (Bilinmeyen Vaka)
        Console.WriteLine($"[INTENT-LLM] Yerel eşleşme yok, Gemini'ye soruluyor...");
        
        // Mevcut Canonical Key'leri LLM'ye bildiriyoruz ki rastgele bir şey uydurmasın.
        var canonicalKeys = string.Join(", ", _synonymMap.Keys);
        var llmResult = await _gemini.ResolveIntentCanonicalAsync(userQuery, canonicalKeys);

        if (string.IsNullOrWhiteSpace(llmResult))
            throw new Exception($"Intent çözümlenemedi. Lütfen 'tc no', 'hasta adı' gibi daha net belirtin.");

        Console.WriteLine($"[INTENT-LLM] '{userQuery}' -> Canonical Key: '{llmResult}'");
        return (llmResult.Trim().ToUpperInvariant(), "llm");
    }

    private static string? TryLocalMapping(string query)
    {
        var words = Tokenize(query);
        var cleanQuery = string.Join(" ", words).ToLowerInvariant();

        foreach (var (canonicalField, synonyms) in _synonymMap)
        {
            // Kullanıcının yazdığı metnin (veya kelimenin) herhangi bir synonym ile eşleşip eşleşmediğini kontrol ediyoruz.
            // Örneğin: cleanQuery="tc no", synonym="tc no" -> Match!
            foreach (var synonym in synonyms)
            {
                if (cleanQuery.Contains(synonym.ToLowerInvariant()) || synonym.ToLowerInvariant().Contains(cleanQuery))
                    return canonicalField;
            }
        }
        return null;
    }

    private static List<string> Tokenize(string query)
    {
        return Regex.Split(query.Trim(), @"\s+")
                    .Where(w => w.Length > 1 && !_stopWords.Contains(w))
                    .ToList();
    }
}
