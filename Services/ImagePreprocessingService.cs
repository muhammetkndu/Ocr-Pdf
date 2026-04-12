using ImageMagick;

namespace ocrProje.Services;

public class ImagePreprocessingService
{
    private const int MinLongEdgePx = 2000;
    private const double MaxUpscaleFactor = 2.5;

    /// <summary>
    /// Gri tonlama, kontrast, hafif gürültü azaltma, deskew ve (gerekirse) upscaling ile OCR girişini iyileştirir.
    /// </summary>
    public string PreprocessImageForOcr(string originalImagePath)
    {
        string directory = Path.GetDirectoryName(originalImagePath) ?? Path.GetTempPath();
        string filename = "preprocessed_" + Path.GetFileName(originalImagePath);
        string newPath = Path.Combine(directory, filename);

        using (var image = new MagickImage(originalImagePath))
        {
            image.Grayscale();

            image.Despeckle();

            image.AutoLevel();

            try
            {
                image.Deskew(new Percentage(40));
            }
            catch
            {
                // Bazı ortamlarda deskew istisna verebilir; OCR yine çalışsın.
            }

            UpscaleIfTooSmall(image);

            image.Write(newPath);
        }

        Console.WriteLine($"[PREPROCESS] Görüntü işlendi (kontrast+deskew+min boyut): {newPath}");
        return newPath;
    }

    private static void UpscaleIfTooSmall(MagickImage image)
    {
        var w = (double)image.Width;
        var h = (double)image.Height;
        var longEdge = Math.Max(w, h);
        if (longEdge < MinLongEdgePx && longEdge > 50)
        {
            var f = Math.Min(MaxUpscaleFactor, MinLongEdgePx / longEdge);
            var nw = (uint)Math.Round(w * f);
            var nh = (uint)Math.Round(h * f);
            image.FilterType = FilterType.Lanczos;
            image.Resize(new MagickGeometry(nw, nh));
            Console.WriteLine($"[PREPROCESS] Upscaling {f:P0} → {nw}x{nh}");
        }
    }
}
