using ImageMagick;
using ocrProje.Models;

namespace ocrProje.Services;

public class SmartCropService
{
    /// <summary>
    /// Bulunan Çıpanın (Anchor) koordinatını alıp, o satırı yatay olarak en sağa kadar uzatıp kırpar.
    /// Sonucu Base64 formatında ve kırpılan bölgenin OCR metni olarak döndürür.
    /// </summary>
    public (string Base64, string CroppedOcrText) CropAnchorRowAsBase64(string imagePath, AnchorCoordinate anchorCoord)
    {
        using var magickImg = new MagickImage(imagePath);
        int imgWidth = (int)magickImg.Width;
        int imgHeight = (int)magickImg.Height;

        // Çıpanın bulunduğu kutu
        var box = anchorCoord.Box;

        // Biraz Padding ekleyelim (alt ve üst satırın bir kısmını kapsasın, ufak OCR hataları tolere edilsin)
        const int padY = 30; // yüksek tolerans
        
        // BAĞLAM GENİŞLETME (X = 0):
        // Çıpanın ne olduğunu LLM'in tam görmesi (bağlam kopmaması) için en soldan başlatıyoruz.
        int x = 0; 
        int y = Math.Max(0, box.Y1 - padY);
        
        // Genişlik her halükarda dokümanın sağına kadar uzatılacak! (Tablo yapılarındaki değerleri garantiye almak için)
        int right = imgWidth;
        // Yüksekliği kendi boyutuna + alt tarafa bir miktar tolerans olarak ekleyelim (çünkü değer alt satırda da olabilir)
        int bottom = Math.Min(imgHeight, box.Y2 + padY + 40); 

        int w = right - x;
        int h = bottom - y;

        Console.WriteLine($"[SMART CROP] Anchor: '{anchorCoord.AnchorWord}' için satır kırpılıyor... X:{x} Y:{y} W:{w} H:{h} (Resim Genişliği: {imgWidth})");

        var geometry = new MagickGeometry(x, y, (uint)w, (uint)h) { IgnoreAspectRatio = true };
        magickImg.Crop(geometry);

        string croppedPath = imagePath + "_cropped.png";
        magickImg.Write(croppedPath);

        string croppedOcrText = "";
        try 
        {
            // Kırpılan böngenin tam metnini Tesseract ile geri oku (LLM ile benzerlik taraması için)
            using var engine = new Tesseract.TesseractEngine(Path.Combine(Environment.CurrentDirectory, "tessdata"), "tur", Tesseract.EngineMode.Default);
            using var pix = Tesseract.Pix.LoadFromFile(croppedPath);
            using var page = engine.Process(pix);
            croppedOcrText = page.GetText()?.Trim() ?? "";
        }
        catch { }

        if (File.Exists(croppedPath)) File.Delete(croppedPath);

        return (magickImg.ToBase64(MagickFormat.Png), croppedOcrText);
    }
}
