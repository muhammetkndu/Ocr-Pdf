using Microsoft.AspNetCore.Mvc;
using ocrProje.Models;
using ocrProje.Services;
using Microsoft.Extensions.Caching.Memory;

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

builder.Services.AddMemoryCache();

var app = builder.Build();

// 2. wwwroot içindeki html/css dosyalarımızı dışarı sunabilmek için
app.UseStaticFiles();

// ==========================================
// AŞAMA 1: DOSYA YÜKLEME VE TAM TARAMA (UPLOAD & INDEX)
// ==========================================
app.MapPost("/api/upload", async (
    [FromServices] PdfConverterService         pdfService,
    [FromServices] ImagePreprocessingService   prepService,
    [FromServices] DocumentClassifierService   classifierService,
    [FromServices] AnchorDetectorService       anchorService,
    [FromServices] Microsoft.Extensions.Caching.Memory.IMemoryCache cache,
    [FromForm] IFormFile file) => 
{
    if (file == null || file.Length == 0) return Results.BadRequest("Dosya yüklenmedi.");

    // Guid ile oturuma ozel dosya ismi (Ayni anda birden cok dosyanin cakismasini onler)
    string sessionId = Guid.NewGuid().ToString();
    var ext = Path.GetExtension(file.FileName);
    var tempFilePath = Path.Combine(Path.GetTempPath(), $"{sessionId}{ext}");

    using (var stream = new FileStream(tempFilePath, FileMode.Create))
        await file.CopyToAsync(stream);

    try
    {
        string imagePath = tempFilePath;

        if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            imagePath = pdfService.ConvertFirstPageToImage(tempFilePath);

        string preprocessedImage = prepService.PreprocessImageForOcr(imagePath);
        string docType = classifierService.ClassifyDocument(preprocessedImage);

        // Belgeyi baştan sona RAM'e tarayıp alıyoruz.
        var allLines = anchorService.ScanEntireDocument(preprocessedImage);

        // RAM'e session olarak at
        var session = new DocumentSession
        {
            SessionId = sessionId,
            OriginalFilePath = tempFilePath,
            PreprocessedImagePath = preprocessedImage,
            DocType = docType,
            ScannedLines = allLines,
            CreatedAt = DateTime.UtcNow
        };

        var cacheOptions = new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(20))
            .RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                // Süresi dolunca temp resimleri sil!
                if (value is DocumentSession s)
                {
                    if (System.IO.File.Exists(s.OriginalFilePath)) System.IO.File.Delete(s.OriginalFilePath);
                    if (s.OriginalFilePath != s.PreprocessedImagePath && System.IO.File.Exists(s.PreprocessedImagePath)) 
                        System.IO.File.Delete(s.PreprocessedImagePath);
                }
            });

        cache.Set(sessionId, session, cacheOptions);

        return Results.Ok(new { sessionId, docType, message = "Belge başarıyla haritalandı." });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Yükleme aşamasında hata: {ex.Message}");
    }
}).DisableAntiforgery();

// ==========================================
// AŞAMA 2: SORGULAMA (QUERY - Işık Hızında)
// ==========================================
app.MapPost("/api/query", async (
    [FromServices] OcrService                  ocrService,
    [FromServices] GeminiService               geminiService,
    [FromServices] SimilarityService           similarityService,
    [FromServices] IntentResolverService       intentService,
    [FromServices] AnchorDetectorService       anchorService,
    [FromServices] SmartCropService            cropService,
    [FromServices] Microsoft.Extensions.Caching.Memory.IMemoryCache cache,
    [FromForm] string sessionId,
    [FromForm] string userQuery) =>
{
    if (string.IsNullOrWhiteSpace(sessionId)) return Results.BadRequest("Session ID boş!");
    if (string.IsNullOrWhiteSpace(userQuery)) return Results.BadRequest("Soru boş!");

    if (!cache.TryGetValue(sessionId, out var sessionObj) || sessionObj is not DocumentSession session)
    {
        return Results.BadRequest("Oturum zaman aşımına uğramış veya geçersiz. Lütfen belgeyi tekrar yükleyin.");
    }

    try
    {
        // ── 1. AŞAMA: Intent Çözümleme ────────────────────────────────────────
        var cleanQuery = intentService.GetCleanQuery(userQuery); 
        var (resolvedField, intentMethod) = await intentService.ResolveAsync(userQuery, session.DocType);
        Console.WriteLine($"[INTENT] '{userQuery}' -> İç Eşleşme (Çıpa Taraması İçin): '{resolvedField}'");

        // ── 2. AŞAMA: RAM'den Çıpa Tespiti (Memory Search - OCR GEREKTİRMEZ) ──
        var (anchorCoord, anchorNote) = anchorService.FindAnchorInMemory(session.ScannedLines, session.DocType, resolvedField);
        
        string base64Image;
        string ocrLineText = "";

        if (anchorCoord != null)
        {
            // ── 3. AŞAMA: Dinamik Kırpma (Disk'teki hazır resimden) ─────────────
            var cropResult = cropService.CropAnchorRowAsBase64(session.PreprocessedImagePath, anchorCoord);
            base64Image = cropResult.Base64;
            ocrLineText = cropResult.CroppedOcrText; // Tümüyle kırpık bölgenin okunmuş hali
        }
        else
        {
            Console.WriteLine("[UYARI] Hafızada Hedef veri için Çıpa bulunamadı. Heuristic metoda geri düşülüyor (oturum OCR satırları, tam sayfa yeniden tarama yok).");
            var sessionLines = AnchorDetectorService.ToLineTuples(session.ScannedLines);
            var fallbackResult = ocrService.CropByField(session.PreprocessedImagePath, sessionLines, resolvedField, session.DocType);
            base64Image = fallbackResult.Base64;
            ocrLineText = fallbackResult.OcrLineText;
        }

        // ── 4. AŞAMA: Gemini'ye Kırpılmış Görüntüyü Gönder ────────────────────
        var (jsonResult, inputTokens, outputTokens) =
            await geminiService.GetJsonFromImageAsync(base64Image, cleanQuery);

        // ── 5. AŞAMA: Benzerlik Analizi ───────────────────────────────────────
        string llmValue = similarityService.ExtractValueFromJson(jsonResult);
        double score    = similarityService.ComputeSimilarity(ocrLineText, llmValue);
        bool   isMatch  = similarityService.IsMatch(score);

        Console.WriteLine($"[BENZERLİK] OCR:'{ocrLineText}'  LLM:'{llmValue}'  Score:{score:P1}  Match:{isMatch}");

        // Dosya silmiyoruz çünkü Session hala aktif (15 dk ömrü var)

        return Results.Ok(new ExtractionResponse
        {
            // Intent
            UserQuery          = userQuery,
            CanonicalField     = resolvedField,
            UserQueryClean     = cleanQuery,
            ResolvedField      = cleanQuery,
            IntentMethod       = intentMethod,
            AnchorMatchNote    = anchorNote,

            // Temel
            FieldName          = resolvedField,
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