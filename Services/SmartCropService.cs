using ImageMagick;
using Tesseract;

namespace ocrProje.Services;

public class SmartCropService
{
    /// <summary>
    /// Çıpa satırını (isteğe bağlı kelime seviyesinde soldan daraltarak) kırpar, Base64 ve kırpık OCR metnini döndürür.
    /// </summary>
    public (string Base64, string CroppedOcrText) CropAnchorRowAsBase64(string imagePath, AnchorCoordinate anchorCoord)
    {
        using var fullImg = new MagickImage(imagePath);
        int imgWidth = (int)fullImg.Width;
        int imgHeight = (int)fullImg.Height;
        var box = anchorCoord.Box;

        const int padY = 30;
        int y = Math.Max(0, box.Y1 - padY);
        int bottom = Math.Min(imgHeight, box.Y2 + padY + 40);
        int h = bottom - y;

        int xStart = 0;
        var stripPath = Path.Combine(Path.GetTempPath(), $"ocr_strip_{Guid.NewGuid():N}.png");
        try
        {
            using (var stripImg = new MagickImage(imagePath))
            {
                var stripGeom = new MagickGeometry(0, y, (uint)imgWidth, (uint)h) { IgnoreAspectRatio = true };
                stripImg.Crop(stripGeom);
                stripImg.Write(stripPath);
            }

            var endX = TryFindAnchorEndXOnStrip(stripPath, anchorCoord.AnchorWord);
            if (endX.HasValue && endX.Value >= 0 && endX.Value < imgWidth)
            {
                const int leftPad = 8;
                xStart = Math.Min(imgWidth - 1, Math.Max(0, endX.Value + leftPad));
                Console.WriteLine($"[SMART CROP] Kelime seviyesi: sol X={xStart} (çıpa '{anchorCoord.AnchorWord}')");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SMART CROP] Kelime seviyesi çıpa araması atlandı: {ex.Message}");
        }
        finally
        {
            TryDelete(stripPath);
        }

        int w = imgWidth - xStart;
        Console.WriteLine($"[SMART CROP] Anchor: '{anchorCoord.AnchorWord}' kırpım X:{xStart} Y:{y} W:{w} H:{h}");

        using var magickImg = new MagickImage(imagePath);
        var geometry = new MagickGeometry(xStart, y, (uint)w, (uint)h) { IgnoreAspectRatio = true };
        magickImg.Crop(geometry);

        var croppedPath = imagePath + "_cropped.png";
        magickImg.Write(croppedPath);

        string croppedOcrText = "";
        try
        {
            using var engine = new TesseractEngine(Path.Combine(Environment.CurrentDirectory, "tessdata"), "tur", EngineMode.Default);
            using var pix = Pix.LoadFromFile(croppedPath);
            using var page = engine.Process(pix);
            croppedOcrText = page.GetText()?.Trim() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SMART CROP] Kırpık bölge OCR hatası: {ex.Message}");
        }

        TryDelete(croppedPath);

        return (magickImg.ToBase64(MagickFormat.Png), croppedOcrText);
    }

    /// <summary>
    /// Yatay şerit üzerinde çıpa ile örtüşen kelimenin sağ kenarını bulur (tam görüntü X koordinatı ile uyumludur).
    /// </summary>
    private static int? TryFindAnchorEndXOnStrip(string stripPath, string anchorWord)
    {
        var anchorLower = anchorWord.ToLowerInvariant();
        if (string.IsNullOrEmpty(anchorLower))
            return null;

        var tessDir = Path.Combine(Environment.CurrentDirectory, "tessdata");
        using var engine = new TesseractEngine(tessDir, "tur", EngineMode.Default);
        using var pix = Pix.LoadFromFile(stripPath);
        using var page = engine.Process(pix);

        var bestEnd = -1;
        using (var iter = page.GetIterator())
        {
            iter.Begin();
            do
            {
                var w = iter.GetText(PageIteratorLevel.Word)?.Trim().ToLowerInvariant() ?? "";
                if (w.Length == 0)
                    continue;

                var hit = w.Contains(anchorLower, StringComparison.Ordinal)
                          || (anchorLower.Length >= 3 && anchorLower.Contains(w, StringComparison.Ordinal) && w.Length >= 2);
                if (hit && iter.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                    bestEnd = Math.Max(bestEnd, rect.X2);
            } while (iter.Next(PageIteratorLevel.Word));
        }

        return bestEnd >= 0 ? bestEnd : null;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            /* ignore */
        }
    }
}
