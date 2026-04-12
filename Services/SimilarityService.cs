using System.Text.Json;
using System.Text.RegularExpressions;

namespace ocrProje.Services;

/// <summary>
/// OCR satır metni ile LLM JSON çıktısı arasındaki benzerliği ölçer.
/// Herhangi bir alan için çalışır (IBAN, fatura no, hasta adı, vb.)
/// </summary>
public class SimilarityService
{
    private const double MatchThreshold = 0.85;

    /// <summary>
    /// LLM JSON string'inden ({"iban": "TR87..."}) değeri çıkar.
    /// Tek-key JSON varsayımı: ilk değer alınır.
    /// </summary>
    public string ExtractValueFromJson(string jsonText)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonText);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Null)
                    return "YOK_VEYA_ESLESMEDI";

                var val = prop.Value.GetString() ?? "";
                if (!string.IsNullOrWhiteSpace(val))
                    return val.Trim();
            }
        }
        catch { /* JSON değilse ham metni döndür */ }

        return jsonText.Trim();
    }

    /// <summary>
    /// LLM'in bulduğu değeri (kısa), OCR metninin (uzun) içinde sliding window ile arar.
    /// 
    /// Neden sliding window?
    ///   OCR satırı: "Hesap/K.Kart No IBAN Açıklama B/A ... TR0F7 0011 ... TL 605.00"  (~80 karakter)
    ///   LLM değeri: "TR87 0006 2000 3440 0006 2986 52"                                 (~26 karakter)
    ///   Tam satır Levenshtein → uzunluk farkı skoru mahveder.
    ///   Sliding window → LLM değerini OCR içinde arar, en iyi pencereyi bulur → doğru skor.
    /// </summary>
    public double ComputeSimilarity(string ocrText, string llmValue)
    {
        if (llmValue == "YOK_VEYA_ESLESMEDI") return 0.0;
        if (string.IsNullOrWhiteSpace(ocrText) && string.IsNullOrWhiteSpace(llmValue)) return 1.0;
        if (string.IsNullOrWhiteSpace(ocrText) || string.IsNullOrWhiteSpace(llmValue)) return 0.0;

        var na = Normalize(ocrText);   // tüm OCR satırı — normalize
        var nb = Normalize(llmValue);  // LLM'in bulduğu kısa değer

        if (na == nb) return 1.0;
        if (nb.Length == 0) return 0.0;

        // LLM değeri OCR metninden kısaysa → sliding window ile OCR içinde ara
        if (nb.Length < na.Length)
            return BestSubstringSimilarity(na, nb);

        // Eşit veya LLM daha uzunsa → klasik normalize Levenshtein
        return NormalizedLevenshtein(na, nb);
    }

    public bool IsMatch(double score) => score >= MatchThreshold;

    // ── Sliding Window ────────────────────────────────────────────────────────

    /// <summary>
    /// OCR metni içinde LLM değeriyle en iyi eşleşen pencereyi bulur.
    /// Pencere boyutu: pattern uzunluğu ve +1/+2/+3 toleranslı varyantlar denenir.
    /// </summary>
    private static double BestSubstringSimilarity(string text, string pattern)
    {
        double best   = 0.0;
        int    pLen   = pattern.Length;

        // ±3 karakter toleranslı pencere boyutları dene (OCR harf atlama/ekleme hataları için)
        for (int extra = 0; extra <= 3; extra++)
        {
            int winSize = pLen + extra;
            if (winSize > text.Length) break;

            for (int i = 0; i <= text.Length - winSize; i++)
            {
                var    window = text.Substring(i, winSize);
                double score  = NormalizedLevenshtein(window, pattern);

                if (score > best) best = score;
                if (best >= 1.0) return 1.0; // erken çıkış — mükemmel eşleşme
            }
        }

        return best;
    }

    // ── Yardımcılar ───────────────────────────────────────────────────────────

    /// <summary>Boşlukları kaldır, büyük harfe çevir.</summary>
    private static string Normalize(string s)
        => Regex.Replace(s.ToUpperInvariant(), @"\s+", "");

    private static double NormalizedLevenshtein(string a, string b)
    {
        if (a == b) return 1.0;
        int dist   = LevenshteinDistance(a, b);
        int maxLen = Math.Max(a.Length, b.Length);
        return maxLen == 0 ? 1.0 : 1.0 - (double)dist / maxLen;
    }

    private static int LevenshteinDistance(string s, string t)
    {
        int m = s.Length, n = t.Length;
        var dp = new int[m + 1, n + 1];

        for (int i = 0; i <= m; i++) dp[i, 0] = i;
        for (int j = 0; j <= n; j++) dp[0, j] = j;

        for (int i = 1; i <= m; i++)
        for (int j = 1; j <= n; j++)
        {
            int cost = s[i - 1] == t[j - 1] ? 0 : 1;
            dp[i, j] = Math.Min(
                Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                dp[i - 1, j - 1] + cost
            );
        }

        return dp[m, n];
    }
}
