using ImageMagick;

namespace ocrProje.Services;

public class PdfConverterService
{
    public string ConvertFirstPageToImage(string pdfPath)
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

        var settings = new MagickReadSettings
        {
            // 400 DPI: OCR ve kutu doğruluğu için önerilen üst band (dosya boyutu artar).
            Density = new Density(400, 400)
        };

        try
        {
            using var images = new MagickImageCollection();
            images.Read(pdfPath + "[0]", settings); // [0] = sadece ilk sayfayı oku
            var firstPage = images[0];
            firstPage.Format = MagickFormat.Png;
            firstPage.Write(outputPath);
        }
        catch (MagickDelegateErrorException ex)
        {
            // Magick.NET PDF okumak için Ghostscript'e ihtiyaç duyar.
            // Windows'ta: https://www.ghostscript.com/download/gsdnld.html adresinden
            // kurulum yapılması gerekmektedir.
            throw new Exception(
                "PDF dosyası resme çevrilemedi. Muhtemelen Ghostscript yüklü değil. " +
                "https://www.ghostscript.com/download/gsdnld.html adresinden kurun. " +
                $"Orijinal hata: {ex.Message}", ex);
        }

        return outputPath;
    }
}