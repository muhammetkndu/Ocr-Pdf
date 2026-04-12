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
    /// Hafızadaki taranmış satırlar üzerinden Çıpa bulur. Tesseract ÇALIŞTIRMAZ. 
    /// İnanılmaz Hızlıdır (0.001ms).
    /// </summary>
    public AnchorCoordinate? FindAnchorInMemory(List<AnchorCoordinate> allLines, string docType, string targetField)
    {
        // 1. Hedef alan için uygun çıpa kelimesini bulalım. 
        HashSet<string> anchorsToCheck = new(StringComparer.OrdinalIgnoreCase);
        string targetAnchor = targetField.ToLowerInvariant();
        
        // Önce IntentResolver'dan eş anlamlılarını ekliyoruz ("doktor adı" ise sadece kendisi, "TC" ise "tckn" vs. döner)
        var synonyms = IntentResolverService.GetSynonyms(targetField, docType);
        foreach (var syn in synonyms) anchorsToCheck.Add(syn);

        // STRICT ANCHOR FILTERING
        // Sistem jenerik kelimelere atlamasın diye filtre, ancak senin listende "il", "ad", "yaş" gibi kısa hedefler de var.
        var allowList = new string[] { "iban", "vkn", "tckn", "tc", "kdv", "tax", "il", "ad", "yaş", "tel", "cep", "mah", "mah.", "ky.", "no", "m2" };

        var strictAnchors = new List<string>();
        foreach (var a in anchorsToCheck)
        {
            // Eğer aranan kelime allow listesindeyse VEYA en az 3 harfliyse VEYA içinde boşluk varsa ("ad soyad" gibi) aramaya izin ver
            if (allowList.Contains(a)) 
            {
                strictAnchors.Add(a);
            }
            else if (a.Contains(" ") || a.Length >= 3) 
            {
                strictAnchors.Add(a);
            }
        }
        
        // Eğer her şey filtrelendiyse güvenlik amaçlı ilk kelimeyi geri ekle
        if (!strictAnchors.Any())
        {
            strictAnchors.Add(anchorsToCheck.First());
        }

        // OCR MOTORU YOK - Hafızadaki Satırlarda Geziyoruz
        foreach (var line in allLines)
        {
            string lineText = line.LineText;

            // Satırın içinde çıpalardan biri var mı?
            foreach (var anchor in strictAnchors)
            {
                string cleanAnchor = anchor.ToLowerInvariant();
                // Ufak Regex veya Contains ile arama
                if (lineText.Contains(cleanAnchor))
                {
                    Console.WriteLine($"[ANCHOR DETECTED IN MEMORY] Bulundu: {cleanAnchor} satırı: '{lineText}' Koordinat: X:{line.Box.X1} Y:{line.Box.Y1}");
                    
                    return new AnchorCoordinate 
                    { 
                        AnchorWord = cleanAnchor, 
                        Box = line.Box, 
                        LineText = lineText 
                    };
                }
            }
        }

        return null;
    }
}
