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

    // Yerel eşanlamlı haritası artık Constants/IntentConfigData.cs dosyasından çekilecektir!

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
    /// Verilen canonical alan adı ve belge türü için bilinen tüm eşanlamlıları döner.
    /// </summary>
    public static string[] GetSynonyms(string canonicalField, string docType)
    {
        var config = Constants.IntentConfigData.IntentConfig.ContainsKey(docType) 
            ? Constants.IntentConfigData.IntentConfig[docType] 
            : Constants.IntentConfigData.IntentConfig["DIGER"];

        if (config.TryGetValue(canonicalField, out var syns))
            return syns;
        
        return new[] { canonicalField }; 
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
    /// Kullanıcının serbest metin sorgusunu belgenin türüne göre (DocType) 'Canonical Key'e eşler.
    /// </summary>
    public async Task<(string ResolvedField, string Method)> ResolveAsync(string userQuery, string docType)
    {
        if (string.IsNullOrWhiteSpace(userQuery))
            throw new ArgumentException("Sorgu boş olamaz.");

        // 1. LOCAL MAP KONTROLÜ (En Güvenli ve En Hızlı)
        var localCandidate = TryLocalMapping(userQuery, docType);
        if (localCandidate != null)
        {
            Console.WriteLine($"[INTENT-LOCAL] '{userQuery}' -> Canonical Key: '{localCandidate}'");
            return (localCandidate, "local");
        }

        // 2. LLM FALLBACK (Bilinmeyen Vaka)
        Console.WriteLine($"[INTENT-LLM] Yerel eşleşme yok, Gemini'ye soruluyor...");
        
        // Mevcut Canonical Key'leri LLM'ye bildiriyoruz
        var config = Constants.IntentConfigData.IntentConfig.ContainsKey(docType) 
            ? Constants.IntentConfigData.IntentConfig[docType] 
            : Constants.IntentConfigData.IntentConfig["DIGER"];
        
        var canonicalKeys = string.Join(", ", config.Keys);
        var llmResult = await _gemini.ResolveIntentCanonicalAsync(userQuery, canonicalKeys);

        if (string.IsNullOrWhiteSpace(llmResult))
            throw new Exception($"Intent çözümlenemedi. Lütfen 'tc no', 'hasta adı' gibi daha net belirtin.");

        var normalized = llmResult.Trim().ToUpperInvariant();
        if (!config.ContainsKey(normalized))
            throw new Exception($"Intent çözümü geçersiz: '{normalized}'. Lütfen alanı daha net yazın.");

        Console.WriteLine($"[INTENT-LLM] '{userQuery}' -> Canonical Key: '{normalized}'");
        return (normalized, "llm");
    }

    // Kısa eşanlamlılar: yalnızca cleanQuery içinde ayrı bir sözcük olarak geçerse yerel eşleşmeye izin verilir.
    private static readonly HashSet<string> ShortAmbiguousSynonyms = new(StringComparer.OrdinalIgnoreCase)
    {
        "no", "il", "ad", "tel", "tc", "vd", "ay", "m2", "ky", "mah"
    };

    private static string? TryLocalMapping(string query, string docType)
    {
        var words = Tokenize(query);
        var cleanQuery = string.Join(" ", words).ToLowerInvariant();
        if (string.IsNullOrEmpty(cleanQuery))
            return null;

        var config = Constants.IntentConfigData.IntentConfig.ContainsKey(docType) 
            ? Constants.IntentConfigData.IntentConfig[docType] 
            : Constants.IntentConfigData.IntentConfig["DIGER"];

        var matches = new List<(string Field, int Quality)>();

        foreach (var (canonicalField, synonyms) in config)
        {
            foreach (var synonym in synonyms)
            {
                var s = synonym.ToLowerInvariant();
                if (s.Length < 2)
                    continue;
                if (!PassesShortSynonymGate(s, cleanQuery))
                    continue;

                var q = MatchQuality(cleanQuery, s);
                if (q <= 0)
                    continue;
                matches.Add((canonicalField, q));
            }
        }

        if (matches.Count == 0)
            return null;

        var perFieldBest = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var (field, q) in matches)
        {
            if (!perFieldBest.TryGetValue(field, out var cur) || q > cur)
                perFieldBest[field] = q;
        }

        var globalMax = perFieldBest.Values.Max();
        var winners = perFieldBest.Where(kv => kv.Value == globalMax).Select(kv => kv.Key).Distinct().ToList();
        if (winners.Count != 1)
        {
            Console.WriteLine($"[INTENT-LOCAL] Belirsizlik: {winners.Count} canonical aday eşit skor; LLM'e bırakılıyor.");
            return null;
        }

        return winners[0];
    }

    private static int MatchQuality(string cleanQuery, string synonymLower)
    {
        if (cleanQuery == synonymLower)
            return 100_000 + synonymLower.Length;
        if (cleanQuery.Contains(synonymLower, StringComparison.Ordinal))
            return 50_000 + synonymLower.Length * 100;
        if (synonymLower.Contains(cleanQuery, StringComparison.Ordinal))
            return 10_000 + cleanQuery.Length * 100;
        return 0;
    }

    private static bool PassesShortSynonymGate(string synonymLower, string cleanQuery)
    {
        if (synonymLower.Length > 3)
            return true;
        if (!ShortAmbiguousSynonyms.Contains(synonymLower))
            return true;

        var tokens = Regex.Split(cleanQuery.Trim(), @"\s+")
            .Where(t => t.Length > 0)
            .Select(NormalizeTokenForGate)
            .ToList();

        return tokens.Any(t => t.Equals(synonymLower, StringComparison.Ordinal));
    }

    private static string NormalizeTokenForGate(string token)
    {
        var t = token.ToLowerInvariant();
        t = Regex.Replace(t, @"^[^\p{L}\p{N}]+", "");
        t = Regex.Replace(t, @"[^\p{L}\p{N}]+$", "");
        return t;
    }

    private static List<string> Tokenize(string query)
    {
        return Regex.Split(query.Trim(), @"\s+")
                    .Where(w => w.Length > 1 && !_stopWords.Contains(w))
                    .ToList();
    }
}
