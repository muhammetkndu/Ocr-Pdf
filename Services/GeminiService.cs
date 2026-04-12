using Google.GenAI;
using Google.GenAI.Types;

namespace ocrProje.Services;

public class GeminiService
{
    private readonly string _apiKey;
    // Kota tablosuna göre en iyi ücretsiz model: RPD 500, RPM 15
    private const string ModelName = "gemini-2.5-flash-lite";

    public GeminiService(IConfiguration config)
    {
        _apiKey = config["GeminiApiKey"] ?? throw new Exception("Gemini API Key eksik! appsettings.Development.json dosyasını kontrol et.");
    }

    /// <summary>
    /// Kırpılmış görüntüyü Gemini'ye gönderir, JSON + token bilgisi döner.
    /// </summary>
    public async Task<(string JsonText, int InputTokens, int OutputTokens)> GetJsonFromImageAsync(
        string base64Image, string targetField)
    {
        var client = new Client(apiKey: _apiKey);

        string jsonKey = targetField.ToLowerInvariant().Replace(" ", "_");
        
        var prompt = 
            $"Sen bir OCR veri doğrulama asistanısın. Kullanıcı senden şu veriyi istiyor: **[{targetField}]**. " +
            $"Sana gönderilen görsel kırpılmış bir belgedir.\n" +
            $"KURAL 1: Görseldeki başlık ile istenen alanın mantıksal olarak uyuştuğundan emin ol. (Örn: İstenen '{targetField}' ise ama görselde tamamen alakasız bir başlık yazıyorsa bunu KESİNLİKLE KABUL ETME).\n" +
            $"KURAL 2: Eğer görselde istenilen alan kesin olarak yoksa veya yanlış bir alan kırpılmışsa, JSON değerine asla uydurma bir veri yazma, `null` döndür.\n" +
            $"Sadece geçerli bir JSON formatında döndür. Başka hiçbir metin veya markdown karakteri (```json vb.) ekleme.\n" +
            $"Örnek Çıktı: {{\"{jsonKey}\": \"bulunan_deger\"}} veya {{\"{jsonKey}\": null}}";

        byte[] imageBytes = Convert.FromBase64String(base64Image);

        var contents = new List<Content>
        {
            new Content
            {
                Role = "user",
                Parts = new List<Part>
                {
                    new Part { Text = prompt },
                    new Part
                    {
                        InlineData = new Blob
                        {
                            MimeType = "image/png",
                            Data     = imageBytes
                        }
                    }
                }
            }
        };

        try
        {
            var response = await client.Models.GenerateContentAsync(
                model:    ModelName,
                contents: contents
            );

            var text         = response.Text ?? "{}";
            var cleanedJson  = text.Replace("```json", "").Replace("```", "").Trim();

            // Token kullanımını oku (API desteklemiyorsa 0 düşer, exception atmaz)
            int inputTokens  = (int)(response.UsageMetadata?.PromptTokenCount     ?? 0);
            int outputTokens = (int)(response.UsageMetadata?.CandidatesTokenCount ?? 0);

            Console.WriteLine($"[TOKEN] Input:{inputTokens}  Output:{outputTokens}  Toplam:{inputTokens + outputTokens}");

            return (cleanedJson, inputTokens, outputTokens);
        }
        catch (Exception ex)
        {
            throw new Exception($"Gemini SDK Hatası: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Kullanıcının serbest metin sorgusunu Canonical Key'lerden birine eşler.
    /// Resim veya OCR metni gönderilmez → sadece basit kelime eşleştirme (en ucuz sorgu).
    /// </summary>
    public async Task<string> ResolveIntentCanonicalAsync(string userQuery, string canonicalKeys)
    {
        var client = new Client(apiKey: _apiKey);

        var prompt =
            $"Sistemde tanımlı standart alanlar (Canonical Keys) şunlardır: [{canonicalKeys}]\n" +
            $"Kullanıcının isteği: \"{userQuery}\"\n" +
            "Soru: Kullanıcının isteği bu standart alanlardan hangisine karşılık geliyor? " +
            "Sadece o alanın adını yaz (Örn: TCKN, TUTAR). Birebir uymasa bile en mantıklısını seç.";

        var contents = new List<Content>
        {
            new Content
            {
                Role  = "user",
                Parts = new List<Part> { new Part { Text = prompt } }
            }
        };

        try
        {
            var response = await client.Models.GenerateContentAsync(
                model:    ModelName,
                contents: contents
            );

            var result = (response.Text ?? "").Trim().ToUpperInvariant();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Intent Canonical çözümleme hatası: {ex.Message}", ex);
        }
    }
}