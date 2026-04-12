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
    /// Belgeyi PageIteratorLevel.TextLine olarak okuyup, içerisinde çıpa (anchor) arar ve bulduğu ilk iyi eşleşmenin koordinatlarını döndürür.
    /// </summary>
    public AnchorCoordinate? FindAnchorCoordinate(string imagePath, string docType, string targetField)
    {
        // 1. Hedef alan için uygun çıpa kelimesini bulalım. 
        HashSet<string> anchorsToCheck = new(StringComparer.OrdinalIgnoreCase);
        string targetAnchor = targetField.ToLowerInvariant();
        
        // Önce IntentResolver'dan eş anlamlılarını ekliyoruz ("doktor adı" ise sadece kendisi, "TC" ise "tckn" vs. döner)
        var synonyms = IntentResolverService.GetSynonyms(targetField);
        foreach (var syn in synonyms) anchorsToCheck.Add(syn);

        var possibleAnchors = Constants.AnchorConfigData.AnchorConfig.ContainsKey(docType) 
            ? Constants.AnchorConfigData.AnchorConfig[docType] 
            : Constants.AnchorConfigData.AnchorConfig["DIGER"];

        // Belge türünün resmi konfigine bakarak, kullanıcının yazdığı terime (veya eşanlamlılarına) benzeyen her çıpayı havuza al:
        // Eğer config'de "doktor" varsa ve bizde "doktor adı" varsa, "doktor" kelimesi havuza eklenecek.
        // Listeyi silmek (overwrite) yerine KAPSAM GENİŞLETİLDİ.
        foreach (var a in possibleAnchors)
        {
            if (anchorsToCheck.Any(syn => syn.Contains(a) || a.Contains(syn)))
            {
                anchorsToCheck.Add(a);
            }
        }

        // STRICT ANCHOR FILTERING
        // Jenerik kelimeleri engellemek için, Çıpa havuzunu filtreliyoruz:
        // İçinde boşluk olmayan (Tek kelime) ve uzunluğu 4 karakterden kısa olan kelimeleri çıkar!
        // İstisna: "iban" ve "vkn", "tc" gibi kanonik alan kısaltmaları kalabilir. Özel izin listesi:
        var allowList = new string[] { "iban", "vkn", "tckn", "tc", "kdv", "tax" };

        var strictAnchors = new List<string>();
        foreach (var a in anchorsToCheck)
        {
            if (allowList.Contains(a)) 
            {
                strictAnchors.Add(a);
            }
            else if (a.Contains(" ") || a.Length >= 4) // En az iki kelime VEYA 4 harften uzun olmalı
            {
                strictAnchors.Add(a);
            }
        }
        
        // Eğer her şey filtrelendiyse güvenlik amaçlı ilk kelimeyi geri ekle
        if (!strictAnchors.Any())
        {
            strictAnchors.Add(anchorsToCheck.First());
        }

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

                // Satırın içinde çıpalardan biri var mı?
                foreach (var anchor in strictAnchors)
                {
                    string cleanAnchor = anchor.ToLowerInvariant();
                    // Ufak Regex veya Contains ile arama
                    if (lineText.Contains(cleanAnchor))
                    {
                        if (iter.TryGetBoundingBox(PageIteratorLevel.TextLine, out Rect rect))
                        {
                            Console.WriteLine($"[ANCHOR DETECTED] Bulundu: {cleanAnchor} satırı: '{lineText.Trim()}' Koordinat: {rect}");
                            
                            // Basit bir heuristics; ilk eşleşmeyi döndür. 
                            // (Eğer daha akıllı olması istenirse en üsttekini veya tam eşleşeni alabiliriz)
                            return new AnchorCoordinate 
                            { 
                                AnchorWord = cleanAnchor, 
                                Box = rect, 
                                LineText = lineText.Trim() 
                            };
                        }
                    }
                }

            } while (iter.Next(PageIteratorLevel.TextLine));
        }

        return null;
    }
}
