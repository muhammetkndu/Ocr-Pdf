using ImageMagick;

namespace ocrProje.Services;

public class ImagePreprocessingService
{
    /// <summary>
    /// Görüntüyü alır, gri tonlamaya çevirir ve (gerekiyorsa binarization ile) OCR başarısını artırmak için işler.
    /// İşlenmiş yeni bir geçici dosyanın yolunu döner.
    /// </summary>
    public string PreprocessImageForOcr(string originalImagePath)
    {
        string directory = Path.GetDirectoryName(originalImagePath) ?? Path.GetTempPath();
        string filename = "preprocessed_" + Path.GetFileName(originalImagePath);
        string newPath = Path.Combine(directory, filename);

        using (var image = new MagickImage(originalImagePath))
        {
            image.Grayscale(); // OCR için çok faydalıdır (Siyah Beyaz algısı)
            // image.Level(new Percentage(10), new Percentage(90)); // opsiyonel, kontrast arttırma
            // image.AutoLevel();
            
            // Eğer resim çok kalitesizse:
            // image.Threshold(new Percentage(50)); // Binarization
            
            image.Write(newPath);
        }

        Console.WriteLine($"[PREPROCESS] Görüntü işlendi: {newPath}");
        return newPath;
    }
}
