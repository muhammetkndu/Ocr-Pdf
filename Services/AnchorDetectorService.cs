using Tesseract;

namespace ocrProje.Services;

public class AnchorCoordinate
{
    public string AnchorWord { get; set; } = "";
    public Rect Box { get; set; }
    public string LineText { get; set; } = "";
}

public class AnchorDetectorService
{
    private readonly string _tessDataPath = Path.Combine(Environment.CurrentDirectory, "tessdata");

    // Belge türlerine göre Çıpa (Anchor) konfigürasyonu Constants/AnchorConfigData.cs dosyasına taşınmıştır.

    /// <summary>
    /// Belgenin tamamını sayfadaki tüm metin bloklarıyla haritalayıp ram'e döndürür. (Bir kere çalışır).
    /// </summary>
    public List<AnchorCoordinate> ScanEntireDocument(string imagePath)
    {
        var result = new List<AnchorCoordinate>();

        using var engine = new TesseractEngine(_tessDataPath, "tur", EngineMode.Default);
        using var img = Pix.LoadFromFile(imagePath);
        using var page = engine.Process(img);

        using (var iter = page.GetIterator())
        {
            iter.Begin();
            do
            {
                string lineText = iter.GetText(PageIteratorLevel.TextLine)?.ToLowerInvariant() ?? "";
                if (string.IsNullOrWhiteSpace(lineText)) continue;

                if (iter.TryGetBoundingBox(PageIteratorLevel.TextLine, out Rect rect))
                {
                    result.Add(new AnchorCoordinate 
                    { 
                        LineText = lineText.Trim(), 
                        Box = rect 
                    });
                }

            } while (iter.Next(PageIteratorLevel.TextLine));
        }

        return result;
    }

    /// <summary>
    /// Oturum satırlarını heuristic kırpma için (string, Rect) listesine çevirir.
    /// </summary>
    public static List<(string Text, Rect Box)> ToLineTuples(List<AnchorCoordinate> scannedLines) =>
        OcrService.ToLineTuplesFromSession(scannedLines);

    /// <summary>
    /// Hafızadaki taranmış satırlar üzerinden Çıpa bulur. Tesseract ÇALIŞTIRMAZ.
    /// Birden fazla aday satırda regex (IBAN/TCKN vb.) ve en uzun eşleşen çıpa metni ile skorlar.
    /// </summary>
    public (AnchorCoordinate? Match, string? SelectionNote) FindAnchorInMemory(
        List<AnchorCoordinate> allLines, string docType, string targetField)
    {
        HashSet<string> anchorsToCheck = new(StringComparer.OrdinalIgnoreCase);
        var synonyms = IntentResolverService.GetSynonyms(targetField, docType);
        foreach (var syn in synonyms) anchorsToCheck.Add(syn);

        var allowList = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "iban", "vkn", "tckn", "tc", "kdv", "tax", "il", "ad", "yaş", "tel", "cep", "mah", "mah.", "ky.", "no", "m2"
        };

        var strictAnchors = new List<string>();
        foreach (var a in anchorsToCheck)
        {
            if (allowList.Contains(a))
                strictAnchors.Add(a);
            else if (a.Contains(' ') || a.Length >= 3)
                strictAnchors.Add(a);
        }

        if (strictAnchors.Count == 0)
            strictAnchors.Add(synonyms[0]);

        var patternRx = OcrService.GetPatternRegexForCanonicalField(targetField);

        var scored = new List<(AnchorCoordinate coord, double score, string matchedAnchor)>();

        foreach (var line in allLines)
        {
            string lineText = line.LineText;
            string? bestOnLine = null;
            var bestLen = 0;
            foreach (var anchor in strictAnchors)
            {
                var cleanAnchor = anchor.ToLowerInvariant();
                if (lineText.Contains(cleanAnchor) && cleanAnchor.Length > bestLen)
                {
                    bestLen = cleanAnchor.Length;
                    bestOnLine = cleanAnchor;
                }
            }

            if (bestOnLine == null) continue;

            double score = bestLen;
            if (patternRx?.IsMatch(lineText) == true)
                score += 1000;
            score -= line.Box.Y1 * 0.001;

            scored.Add((new AnchorCoordinate
            {
                AnchorWord = bestOnLine,
                Box = line.Box,
                LineText = lineText
            }, score, bestOnLine));
        }

        if (scored.Count == 0)
            return (null, null);

        var best = scored
            .OrderByDescending(x => x.score)
            .ThenBy(x => x.coord.Box.Y1)
            .First();

        Console.WriteLine(
            $"[ANCHOR DETECTED IN MEMORY] Seçilen: '{best.matchedAnchor}' skor={best.score:F1} satır='{best.coord.LineText}' Y:{best.coord.Box.Y1}");

        string? note = scored.Count > 1
            ? $"{scored.Count} aday satır; en yüksek skor seçildi (çıpa uzunluğu + varsa desen eşleşmesi)."
            : null;

        return (best.coord, note);
    }
}
