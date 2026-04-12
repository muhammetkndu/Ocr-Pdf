using System.Text.RegularExpressions;
using ImageMagick;
using Tesseract;

namespace ocrProje.Services;

public class OcrService
{
    private readonly string _tessDataPath = Path.Combine(Environment.CurrentDirectory, "tessdata");

    // Canonical alanlara karşılık gelen Regex pattern'leri (Pattern Matching)
    private static readonly Dictionary<string, string> _patternMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["IBAN"]        = @"(?i)TR[0-9A-Za-z\s]{20,30}",
        ["TCKN"]        = @"\b[1-9][0-9]{10}\b",           // 11 haneli T.C. Kimlik
        ["VKN"]         = @"\b[0-9]{10}\b",                // 10 haneli Vergi No
        ["TARIH"]       = @"\b\d{2}[./-]\d{2}[./-]\d{2,4}\b",
        ["TUTAR"]       = @"\b\d{1,3}([.,]\d{3})*[.,]\d{2}\b",
        ["KDV"]         = @"\b\d{1,3}([.,]\d{3})*[.,]\d{2}\b",
        ["FATURA_NO"]   = @"(?i)[A-Z0-9]{16}\b",           // Örn: E-Arşiv GIB2020...
    };

    // ── PUBLIC API ────────────────────────────────────────────────────────────

    public List<(string Text, Rect Box)> ScanAllLines(string imagePath)
    {
        using var engine = new TesseractEngine(_tessDataPath, "tur", EngineMode.Default);
        using var img    = Pix.LoadFromFile(imagePath);
        using var page   = engine.Process(img);

        var lines = new List<(string Text, Rect Box)>();

        using (var iter = page.GetIterator())
        {
            iter.Begin();
            do
            {
                string lineText = iter.GetText(PageIteratorLevel.TextLine) ?? "";
                if (!string.IsNullOrWhiteSpace(lineText) &&
                    iter.TryGetBoundingBox(PageIteratorLevel.TextLine, out Rect rect))
                {
                    lines.Add((lineText.Trim(), rect));
                    // Console.WriteLine($"[OCR Satır]: {lineText.Trim()}");
                }
            } while (iter.Next(PageIteratorLevel.TextLine));
        }

        return lines;
    }

    /// <summary>
    /// Heuristic Scoring (Pattern, Label, Yakınlık) algoritması ile en olası alanı bulur ve kırpar.
    /// </summary>
    public (string Base64, string OcrLineText) CropByField(
        string imagePath,
        List<(string Text, Rect Box)> lines,
        string resolvedField)
    {
        if (lines.Count == 0)
            throw new Exception("Belgede hiç metin bulunamadı.");

        var candidates = ScoreCandidates(lines, resolvedField);
        
        if (candidates.Count == 0)
            throw new Exception($"'{resolvedField}' belgede bulunamadı.");

        // En yüksek puanlı adayı seç (aynı puandaysa belgede daha yukarıdakini (Y) seç)
        var bestCandidate = candidates
            .OrderByDescending(c => c.Score)
            .ThenBy(c => c.CombinedBox.Y1)
            .First();

        Console.WriteLine($"[HEURISTIC] Seçilen Aday: Tür={bestCandidate.WinnerType}, Skor={bestCandidate.Score}, Kutu={bestCandidate.CombinedBox}");

        // ── OCR ham metin birleştir ───────────────────────────────────────
        string ocrLineText = string.Join(" \n", bestCandidate.Lines.Select(l => l.Text)).Trim();

        // ── Kırpma bölgesini hesapla ───────────────────────────────────────
        using var magickImg = new MagickImage(imagePath);
        int imgWidth  = (int)magickImg.Width;
        int imgHeight = (int)magickImg.Height;

        const int pad = 20; // Heuristic aramada toleransı artırmak için padding bir tık büyük
        int x      = Math.Max(0,         bestCandidate.CombinedBox.X1 - pad);
        int y      = Math.Max(0,         bestCandidate.CombinedBox.Y1 - pad);
        int right  = Math.Min(imgWidth,  bestCandidate.CombinedBox.X2 + pad);
        int bottom = Math.Min(imgHeight, bestCandidate.CombinedBox.Y2 + pad);
        
        // Eğer aday SADECE Label ise (Pattern yoksa), değer alt satırlarda olabilir diye alt tarafı ekstra genişletiyoruz
        if (bestCandidate.WinnerType == WinnerType.OnlyLabel)
            bottom = Math.Min(imgHeight, bottom + 50);

        int w = right - x;
        int h = bottom - y;

        Console.WriteLine($"[KIRP] X:{x} Y:{y} W:{w} H:{h}  (Görüntü {imgWidth}x{imgHeight})");

        var geometry = new MagickGeometry(x, y, (uint)w, (uint)h) { IgnoreAspectRatio = true };
        magickImg.Crop(geometry);

        return (magickImg.ToBase64(MagickFormat.Png), ocrLineText);
    }

    public (string Base64, string OcrLineText) GetCroppedImageAsBase64(string imagePath, string targetField)
    {
        var lines = ScanAllLines(imagePath);
        return CropByField(imagePath, lines, targetField);
    }

    // ── HEURISTIC SCORING ALGORITHM ───────────────────────────────────────────

    private enum WinnerType { BothSameLine, LabelAndPatternProximal, OnlyPattern, OnlyLabel }

    private class ExtractionCandidate
    {
        public double Score { get; set; }
        public WinnerType WinnerType { get; set; }
        public Rect CombinedBox { get; set; }
        public List<(string Text, Rect Box)> Lines { get; set; } = new();
    }

    private List<ExtractionCandidate> ScoreCandidates(List<(string Text, Rect Box)> lines, string resolvedField)
    {
        var candidates = new List<ExtractionCandidate>();
        var synonyms = IntentResolverService.GetSynonyms(resolvedField);
        
        _patternMap.TryGetValue(resolvedField, out var patternRegexStr);
        Regex? patternRegex = null;
        if (!string.IsNullOrEmpty(patternRegexStr))
            patternRegex = new Regex(patternRegexStr, RegexOptions.Compiled);

        // 1. Satırların özelliklerini analiz et
        var labelLineIndices = new HashSet<int>();
        var patternLineIndices = new HashSet<int>();

        for (int i = 0; i < lines.Count; i++)
        {
            string raw = lines[i].Text;
            string clean = new string(raw.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

            // Label var mı?
            bool hasLabel = synonyms.Any(syn => 
            {
                var cleanSyn = new string(syn.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
                return clean.Contains(cleanSyn);
            });
            if (hasLabel) labelLineIndices.Add(i);

            // Pattern eşleşiyor mu?
            bool hasPattern = patternRegex?.IsMatch(raw) ?? false;
            if (hasPattern) patternLineIndices.Add(i);
        }

        // 2. Kombinasyon Skoru Hesapla
        for (int i = 0; i < lines.Count; i++)
        {
            bool isLabel = labelLineIndices.Contains(i);
            bool isPattern = patternLineIndices.Contains(i);

            // A. Hem Label Hem Pattern Aynı Satırda (+100 Puan)
            if (isLabel && isPattern)
            {
                candidates.Add(new ExtractionCandidate
                {
                    Score = 100,
                    WinnerType = WinnerType.BothSameLine,
                    CombinedBox = lines[i].Box,
                    Lines = { lines[i] }
                });
                continue;
            }

            // B. Sadece Label Var (+30 Puan baseline)
            if (isLabel)
            {
                // Pattern ile proximity (yakınlık) kontrolü yap.
                // Eğer pattern bu satırın hemen altındaysa!
                if (patternRegex != null)
                {
                    bool foundNearbyPattern = false;
                    // Label'dan sonraki 3 satır içinde Pattern ara
                    for (int j = i + 1; j <= Math.Min(i + 3, lines.Count - 1); j++)
                    {
                        if (patternLineIndices.Contains(j))
                        {
                            // Geometrik Validasyon: Pattern satırı Label'ın altında mı?
                            int dy = lines[j].Box.Y1 - lines[i].Box.Y2;
                            if (dy > -20 && dy < 150) // Aşırı uzak değilse
                            {
                                var combinedBox = Union(lines[i].Box, lines[j].Box);
                                candidates.Add(new ExtractionCandidate
                                {
                                    Score = 80 - (dy * 0.1), // Uzaklaştıkça puan kır
                                    WinnerType = WinnerType.LabelAndPatternProximal,
                                    CombinedBox = combinedBox,
                                    Lines = { lines[i], lines[j] }
                                });
                                foundNearbyPattern = true;
                                break;
                            }
                        }
                    }
                    if (!foundNearbyPattern)
                    {
                        // Sadece label, yanındaki Pattern bulunamadı veya Regex tanımlı değil
                        var box = lines[i].Box;
                        var candidateLines = new List<(string, Rect)> { lines[i] };
                        if (i + 1 < lines.Count)
                        {
                            box = Union(box, lines[i+1].Box); // Eski mantık (label satırı + 1 alt satır guarantee)
                            candidateLines.Add(lines[i+1]);
                        }
                        
                        candidates.Add(new ExtractionCandidate
                        {
                            Score = 30,
                            WinnerType = WinnerType.OnlyLabel,
                            CombinedBox = box,
                            Lines = candidateLines
                        });
                    }
                }
                else
                {
                    // Regex tanımlı değil ("Ad Soyad" vs). Sadece Label ve 1 alt satır al
                    var box = lines[i].Box;
                    var candidateLines = new List<(string, Rect)> { lines[i] };
                    if (i + 1 < lines.Count)
                    {
                        box = Union(box, lines[i+1].Box);
                        candidateLines.Add(lines[i+1]);
                    }
                    candidates.Add(new ExtractionCandidate
                    {
                        Score = 30, // Standart label skoru
                        WinnerType = WinnerType.OnlyLabel,
                        CombinedBox = box,
                        Lines = candidateLines
                    });
                }
            }

            // C. Sadece Pattern Var (Label Yok) (+40 Puan)
            if (isPattern && !isLabel)
            {
                candidates.Add(new ExtractionCandidate
                {
                    Score = 40,
                    WinnerType = WinnerType.OnlyPattern,
                    CombinedBox = lines[i].Box,
                    Lines = { lines[i] }
                });
            }
        }

        return candidates;
    }

    private static Rect Union(Rect a, Rect b)
    {
        int x1 = Math.Min(a.X1, b.X1);
        int y1 = Math.Min(a.Y1, b.Y1);
        int x2 = Math.Max(a.X2, b.X2);
        int y2 = Math.Max(a.Y2, b.Y2);
        return new Rect(x1, y1, x2 - x1, y2 - y1);
    }
}