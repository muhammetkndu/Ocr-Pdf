using System.Text.RegularExpressions;
using ImageMagick;
using Tesseract;

namespace ocrProje.Services;

public class DocumentClassifierService
{
    private readonly string _tessDataPath = Path.Combine(Environment.CurrentDirectory, "tessdata");

    /// <summary>
    /// Görüntünün sadece üst %25'lik kısmını kesip burayı OCR ile okur ve belge türünü tespit eder.
    /// </summary>
    public string ClassifyDocument(string imagePath)
    {
        string cropPath = "fastpass_top_" + Path.GetFileName(imagePath);
        string tempDir = Path.GetTempPath();
        string tempFilePath = Path.Combine(tempDir, cropPath);

        try
        {
            using (var magickImg = new MagickImage(imagePath))
            {
                int imgWidth = (int)magickImg.Width;
                int imgHeight = (int)magickImg.Height;
                int topNavHeight = (int)(imgHeight * 0.25); // %25 lik üst kısım
                
                var geometry = new MagickGeometry(0, 0, (uint)imgWidth, (uint)topNavHeight) { IgnoreAspectRatio = true };
                magickImg.Crop(geometry);
                magickImg.Write(tempFilePath);
            }

            using var engine = new TesseractEngine(_tessDataPath, "tur", EngineMode.Default);
            using var img = Pix.LoadFromFile(tempFilePath);
            using var page = engine.Process(img);
            
            string headerText = page.GetText().ToLowerInvariant();
            
            Console.WriteLine("[FAST-PASS CLASSIFICATION] Header OCR Metni okundu, belge türü tespiti yapılıyor...");
            
            if (headerText.Contains("dekont") || headerText.Contains("transfer") || headerText.Contains("eft") || headerText.Contains("havale"))
            {
                return "DEKONT";
            }
            if (headerText.Contains("fatura") || headerText.Contains("e-arşiv") || headerText.Contains("irsaliye"))
            {
                return "FATURA";
            }
            if (headerText.Contains("mizan") || headerText.Contains("muhasebe"))
            {
                return "MIZAN";
            }

            // Varsayılan
            return "DIGER";
        }
        finally
        {
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }
}
