using Microsoft.AspNetCore.Mvc;
using ocrProje.Models;
using ocrProje.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Servislerimizi sisteme tanıtıyoruz (Dependency Injection)
builder.Services.AddSingleton<PdfConverterService>();
builder.Services.AddSingleton<OcrService>();
builder.Services.AddSingleton<GeminiService>();
builder.Services.AddSingleton<SimilarityService>();
builder.Services.AddSingleton<IntentResolverService>();

// YENI ÇIPA TABANLI SERVISLER
builder.Services.AddSingleton<ImagePreprocessingService>();
builder.Services.AddSingleton<DocumentClassifierService>();
builder.Services.AddSingleton<AnchorDetectorService>();
builder.Services.AddSingleton<SmartCropService>();

var app = builder.Build();

// 2. wwwroot içindeki html/css dosyalarımızı dışarı sunabilmek için
app.UseStaticFiles();

// 3. Ana API Endpoint'imiz (Front-end buraya istek atacak)
app.MapPost("/api/extract", async (
    [FromServices] PdfConverterService         pdfService,
    [FromServices] OcrService                  ocrService,
    [FromServices] GeminiService               geminiService,
    [FromServices] SimilarityService           similarityService,
    [FromServices] IntentResolverService       intentService,
    [FromServices] ImagePreprocessingService   prepService,
    [FromServices] DocumentClassifierService   classifierService,
    [FromServices] AnchorDetectorService       anchorService,
    [FromServices] SmartCropService            cropService,
    [FromForm] IFormFile file,
    [FromForm] string userQuery) =>
{
    if (file == null || file.Length == 0) return Results.BadRequest("Dosya yüklenmedi.");

    var tempFilePath = Path.Combine(Path.GetTempPath(), file.FileName);
    using (var stream = new FileStream(tempFilePath, FileMode.Create))
        await file.CopyToAsync(stream);

    try
    {
        string imagePath = tempFilePath;

        // EĞER PDF İSE: Önce resme çevir
        if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            imagePath = pdfService.ConvertFirstPageToImage(tempFilePath);

        // ── 1. AŞAMA: Intent Çözümleme ────────────────────────────────────────
        var cleanQuery = intentService.GetCleanQuery(userQuery); // Asıl LLM promptu için
        var (resolvedField, intentMethod) = await intentService.ResolveAsync(userQuery);
        Console.WriteLine($"[INTENT] '{userQuery}' -> İç Eşleşme (Çıpa Taraması İçin): '{resolvedField}'");

        // ── 2. AŞAMA: Image Preprocessing (Grayscale vb.) ──────────────────────
        string preprocessedImage = prepService.PreprocessImageForOcr(imagePath);

        // ── 3. AŞAMA: Belge Sınıflandırma (Fast Pass) ─────────────────────────
        string docType = classifierService.ClassifyDocument(preprocessedImage);
        Console.WriteLine($"[DOC-TYPE] Tespit Edilen Belge Türü: {docType}");

        // ── 4. AŞAMA: Smart Anchor Detection ──────────────────────────────────
        // OCR Çıpa araması Canonical Key ile (örn IBAN) tetikleniyor ki synonym haritasından faydalanabilsin.
        var anchorCoord = anchorService.FindAnchorCoordinate(preprocessedImage, docType, resolvedField);
        
        string base64Image;
        string ocrLineText = "";

        if (anchorCoord != null)
        {
            // ── 5. AŞAMA: Dinamik Kırpma (Sağa Doğru Genişletilmiş) ─────────────
            var cropResult = cropService.CropAnchorRowAsBase64(preprocessedImage, anchorCoord);
            base64Image = cropResult.Base64;
            ocrLineText = cropResult.CroppedOcrText; // Tümüyle kırpık bölgenin okunmuş hali
        }
        else
        {
            Console.WriteLine("[UYARI] Hedef veri için Çıpa bulunamadı. Eski Heuristic metoda geri düşülüyor...");
            var allLines = ocrService.ScanAllLines(preprocessedImage);
            var fallbackResult = ocrService.CropByField(preprocessedImage, allLines, resolvedField);
            base64Image = fallbackResult.Base64;
            ocrLineText = fallbackResult.OcrLineText;
        }

        // ── 6. AŞAMA: Gemini'ye Kırpılmış Görüntüyü Gönder ────────────────────
        // LLM'e Canonical Key'i (IBAN) değil, kullanıcının kendi tabirini (hesap no) soruyoruz ki onu döndürsün.
        var (jsonResult, inputTokens, outputTokens) =
            await geminiService.GetJsonFromImageAsync(base64Image, cleanQuery);

        // ── 7. AŞAMA: Benzerlik Analizi ───────────────────────────────────────
        string llmValue = similarityService.ExtractValueFromJson(jsonResult);
        double score    = similarityService.ComputeSimilarity(ocrLineText, llmValue);
        bool   isMatch  = similarityService.IsMatch(score);

        Console.WriteLine($"[BENZERLİK] OCR:'{ocrLineText}'  LLM:'{llmValue}'  Score:{score:P1}  Match:{isMatch}");

        // Geçici dosyaları temizle
        if (System.IO.File.Exists(tempFilePath)) System.IO.File.Delete(tempFilePath);
        if (imagePath != tempFilePath && System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
        if (System.IO.File.Exists(preprocessedImage)) System.IO.File.Delete(preprocessedImage);

        return Results.Ok(new ExtractionResponse
        {
            // Intent
            UserQuery          = userQuery,
            ResolvedField      = cleanQuery, // Ekranda da kullanıcının girdiği kelime görülecek
            IntentMethod       = intentMethod,

            // Temel
            FieldName          = cleanQuery,
            ExtractedValue     = jsonResult,
            CroppedImageBase64 = base64Image,

            // Token
            InputTokens        = inputTokens,
            OutputTokens       = outputTokens,
            TotalTokens        = inputTokens + outputTokens,
            EstimatedCost      = (inputTokens + outputTokens) * (3.75 / 2000000.0),


            // Benzerlik
            OcrLineText        = ocrLineText,
            LlmExtractedText   = llmValue,
            SimilarityScore    = Math.Round(score, 4),
            IsMatch            = isMatch
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Bir hata oluştu knk: {ex.Message}");
    }
}).DisableAntiforgery();

app.MapFallbackToFile("index.html");
app.Run();