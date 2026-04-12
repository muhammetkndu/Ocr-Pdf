namespace ocrProje.Constants;

public static class AnchorConfigData
{
    // 50 Farklı Belge Türü İçin ZENGİNLEŞTİRİLMİŞ Çıpa (Anchor) Konfigürasyonu
    // OCR Hataları, Sektörel Jargon ve Hedef Veri (Hasta Adı, Tutar vb.) Etiketleri Dahil
    public static readonly Dictionary<string, List<string>> AnchorConfig = new(StringComparer.OrdinalIgnoreCase)
    {
        // ==========================================
        // 1. SAĞLIK BELGELERİ (İstirahat, Rapor, Reçete)
        // ==========================================
        ["ISTIRAHAT_RAPORU"] = new List<string> { 
            "hasta adı", "hastanın adı", "adı soyadı", "ad soyad", "sayın", "sn.", "tc kimlik no", "tckn", "kimlik numarası", "sigortalı", "sigortalı sicil no", "doğum tarihi", "baba adı",
            "poliklinik", "servis", "tesis kodu", "sağlık tesisi", "hastane", "kurum",
            "rapor no", "protokol no", "takip no", "vaka türü", "iş kazası", "meslek hastalığı", "hastalık", "analık", 
            "tanı", "teşhis", "ön tanı", "kesin tanı", "icd", "icd-10", "icd kodu", "karar",
            "istirahat başlama", "başlama tarihi", "bitiş tarihi", "işbaşı tarihi", "iş başı", "çalışır", "kontrol", "rapor süresi", "yatarak", "ayakta tedavi",
            "hekim", "doktor", "uzman", "başhekim", "onay", "kaşe", "imza", "diploma no"
        },
        ["EPIKRIZ_RAPORU"] = new List<string> { 
            "epikriz", "çıkış özeti", "hasta adı", "hasta soyadı", "tc no", "dosya no", "protokol numarası", "kabul tarihi", "yatış tarihi", "çıkış tarihi", "taburcu tarihi", 
            "şikayeti", "hikayesi", "özgeçmiş", "soygeçmiş", "fizik muayene", "laboratuvar bulguları", "radyoloji", "yapılan işlemler", "ameliyat", "operasyon", "tedavi", "taburcu", "öneriler", "hekim kaşe"
        },
        ["LABORATUVAR_SONUCU"] = new List<string> { 
            "laboratuvar", "test sonuç", "numune no", "örnek no", "kabul zamanı", "onay zamanı", "hasta adı", "tc kimlik", "yaş", "cinsiyet",
            "parametre", "test adı", "sonuç", "birim", "referans aralığı", "normal değerler", "hemogram", "biyokimya", "hormon", "idrar", "açıklama", "uzm. dr."
        },
        ["RECETE"] = new List<string> { 
            "e-reçete no", "reçete numarası", "hasta tc", "hasta adı soyadı", "kurum sicil no", "tanı", "teşhis", 
            "ilaçlar", "kutu", "adet", "kullanım şekli", "doz", "günde", "tok", "aç", "hekim", "diploma no", "tesis adı"
        },

        // ==========================================
        // 2. BANKACILIK VE FİNANS BELGELERİ
        // ==========================================
        ["DEKONT"] = new List<string> { 
            "tutar", "tutar:", "meblağ", "meblag", "işlem tutarı", "islem tutari", "bakiye", "kalan bakiye", "komisyon", "masraf", "bsmv", "vergi", "işlem hacmi",
            "işlem tarihi", "tarih:", "valör", "valor", "işlem saati", "saat:",
            "iban", "iban:", "tr", "hesap no", "hesap numarası", "müşteri no", "şube kodu", "şube adı",
            "alıcı", "alici", "alıcı adı", "alıcı unvanı", "gönderen", "gonderen", "gönderen unvanı", "tc kimlik", "tckn", "vkn", "vergi no",
            "açıklama", "işlem açıklaması", "referans", "referans no", "dekont no", "sorgu no", "işlem türü", "eft", "havale", "swift", "virman"
        },
        ["KREDI_KARTI_EKSTRESI"] = new List<string> { 
            "sayın", "müşteri no", "kart no", "son 4 hane", "dönem borcu", "donem borcu", "asgari ödeme", "asgari tutar", "ödenmesi gereken", 
            "hesap kesim tarihi", "son ödeme tarihi", "kullanılabilir bakiye", "kredi limiti", "toplam borç", "nakit avans limiti", "önceki bakiye", "devreden bakiye", "alışveriş faizi"
        },
        ["KREDI_SOZLESMESI"] = new List<string> { 
            "kredi tutarı", "kredi limiti", "faiz oranı", "akdi faiz", "temerrüt faizi", "vade", "taksit tutarı", "taksit sayısı", "ödeme planı", 
            "kredi numarası", "kullandırım tarihi", "sözleşme tarihi", "kefil", "asıl borçlu", "müşteri numarası", "tüketici kredisi", "ticari kredi"
        },
        ["POS_SLIBI"] = new List<string> { 
            "işyeri no", "terminal no", "işlem no", "batch no", "sıra no", "prov no", "provizyon", "kart no", "aid", "tutar", "toplam tutar", "şifre ile onaylanmıştır", "temassız", "kredi kartı", "banka kartı", "satış", "iade"
        },
        ["TEMINAT_MEKTUBU"] = new List<string> { 
            "kesin teminat", "geçici teminat", "avans teminat", "mektup tutarı", "azami tutar", "lehtar", "muhatap", "vade tarihi", "süresiz", "kati", "garanti eden", "şube", "banka kaşesi", "imza"
        },
        ["CEK"] = new List<string> { 
            "çek no", "cek no", "keşide tarihi", "keşide yeri", "muhatap banka", "şube", "iban", "ödeyiniz", "emrine", "hamiline", "tutar", "yalnız", "hesap no", "karekod", "seri no"
        },
        ["SENET"] = new List<string> { 
            "vade tarihi", "ödeme tarihi", "kuruş", "tutar", "tanzim tarihi", "tanzim yeri", "borçlu", "kefil", "emrühavalesine", "bedeli malen", "bedeli nakden", "ihtilaf vukuunda", "tc kimlik no", "adres"
        },
        ["POLICE_SIGORTA"] = new List<string> { 
            "poliçe no", "police no", "teklif no", "sigortalı", "sigorta ettiren", "tc no", "vkn", "başlangıç tarihi", "bitiş tarihi", "prim tutarı", "brüt prim", "net prim", "teminat", "muafiyet", "zeyilname", "acente", "acente no", "plaka", "şasi no"
        },

        // ==========================================
        // 3. MUHASEBE VE VERGİ BELGELERİ
        // ==========================================
        ["FATURA"] = new List<string> { 
            "fatura no", "belge no", "ettn", "irsaliye no", "sipariş no", "fatura tarihi", "düzenleme tarihi", "düzenleme saati",
            "sayın", "müşteri", "alıcı", "vkn", "vergi no", "vergi numarası", "tckn", "tc no", "vergi dairesi", "v.d.", "fatura adresi", "teslimat adresi", "mersis", "ticaret sicil",
            "genel toplam", "ödenecek tutar", "toplam tutar", "kdv matrahı", "hesaplanan kdv", "kdv tutarı", "%20", "%10", "%1", "mal hizmet tutarı", "ara toplam", "iskonto", "indirim", "tevkifat", "yazıyla",
            "e-fatura", "e-arşiv", "birim fiyat", "miktar", "karekod"
        },
        ["MIZAN"] = new List<string> { 
            "borç", "borc", "alacak", "bakiye", "borç bakiye", "alacak bakiye", "hesap kodu", "hesap no", "hesap adı", "hesap adi", "ana hesap", "kebir", "alt hesap", 
            "devir", "devreden", "dönem içi", "toplamlar", "genel yekün", "bakiye veriyor"
        },
        ["BORDRO"] = new List<string> { 
            "sicil no", "personel no", "yaka no", "tc kimlik", "ad soyad", "adı soyadı", "sgk no", "sgk sicil", "departman", "unvan", "şube",
            "işe giriş tarihi", "çıkış tarihi", "ay", "yıl", "dönem", "gün sayısı", "çalışılan gün", "sgk günü", "rapor günü", "izin günü",
            "brüt ücret", "net ücret", "ödenecek net", "asgari geçim indirimi", "agi", "ek ödeme", "mesai", "fazla mesai", "ikramiye",
            "gelir vergisi", "damga vergisi", "sgk primi", "işsizlik primi", "bes", "kesintiler toplamı", "kazançlar toplamı"
        },
        ["GIDER_PUSULASI"] = new List<string> { 
            "pusula no", "düzenleme tarihi", "satıcı", "alıcı", "tc kimlik", "adres", "gayrisafi tutar", "gelir vergisi kesintisi", "stopaj", "fon payı", "safi tutar", "net ödenen", "kesinti toplamı", "imza"
        },
        ["SERBEST_MESLEK_MAKBUZU"] = new List<string> { 
            "makbuz no", "smm", "tarih", "brüt tutar", "stopaj", "gelir vergisi", "%20 stopaj", "net alınan", "kdv", "hesaplanan kdv", "tahsil edilen", "hizmetin cinsi", "hizmet bedeli", "müşteri vkn"
        },
        ["TAHSILAT_MAKBUZU"] = new List<string> { 
            "makbuz no", "tahsilat tarihi", "tahsil edilen", "tutar", "yalnız", "yazıyla", "kasa kodu", "tahsilatı yapan", "açıklama", "nakit", "kredi kartı", "çek", "senet"
        },
        ["TEDIYE_MAKBUZU"] = new List<string> { 
            "tediye makbuzu", "ödenen tutar", "ödeme tarihi", "kasa no", "ödemeyi yapan", "teslim alan", "tutar", "açıklama", "imza"
        },
        ["KDV_BEYANNAMESI"] = new List<string> { 
            "beyanname türü", "kdv1", "kdv2", "vergi dairesi", "vergi no", "dönem", "yıl", "ay", "matrah", "hesaplanan kdv", "indirilecek kdv", "ödenecek kdv", "sonraki döneme devreden", "tahakkuk fişi", "onay numarası"
        },
        ["MUHTASAR"] = new List<string> { 
            "muhtasar", "muhsgk", "vergi no", "asgari ücretli", "diğer ücretli", "gayrisafi", "kesilen vergi", "ödenecek vergi", "tahakkuk tutarı", "dönem", "işyeri sicil no"
        },
        ["KURUMLAR_VERGISI"] = new List<string> { 
            "kurumlar vergisi", "ticari bilanço karı", "kanunen kabul edilmeyen", "kkeg", "vergiye tabi kurum kazancı", "hesaplanan kurumlar vergisi", "mahsup edilecek", "ödenecek kurumlar"
        },
        ["GECICI_VERGI"] = new List<string> { 
            "geçici vergi", "gecici", "dönem", "ticari kazanç", "zarar", "ödenecek geçici vergi", "mahsup edilecek geçici vergi", "önceki dönemlerde hesaplanan"
        },
        ["BA_BS_FORMU"] = new List<string> { 
            "ba formu", "bs formu", "mal ve hizmet alımları", "mal ve hizmet satışları", "belge sayısı", "tutar (kdv hariç)", "soyadi/unvani", "vergi kimlik numarası", "ülke kodu"
        },
        ["AMORTISMAN_TABLOSU"] = new List<string> { 
            "iktisadi kıymet", "faydalı ömür", "amortisman oranı", "iktisap tarihi", "iktisap bedeli", "birikmiş amortisman", "net aktif değer", "dönem amortismanı", "gider yeri"
        },

        // ==========================================
        // 4. HUKUK, MAHKEME VE NOTER BELGELERİ
        // ==========================================
        ["NOTER"] = new List<string> { 
            "noterliği", "noteri", "yevmiye no", "yevmiye numarası", "tarih", "düzenleme tarihi", 
            "vekil eden", "vekalet veren", "vekil", "ihtar eden", "muhatap", "borçlu", "alacaklı", "tc kimlik numarası", "vkn", "adres",
            "vekaletname", "ihtarname", "ihbarname", "tasdik", "suret", "aslı gibidir", 
            "harç", "damga vergisi", "değerli kağıt", "makbuz no", "makbuz tutarı"
        },
        ["MAHKEME_KARARI"] = new List<string> { 
            "t.c.", "mahkemesi", "esas no", "karar no", "c.savcısı", "hakim", "katip", "davacı", "davalı", "vekil", "gerekçeli karar", "hüküm", "duruşma tarihi", "karar tarihi", "aidiyetinin tespiti", "muhdesat", "türk milleti adına"
        },
        ["BILIRKISI_RAPORU"] = new List<string> { 
            "bilirkişi raporu", "esas no", "dosya no", "inceleme tarihi", "davacı", "davalı", "keşif", "değerlendirme", "sonuç ve kanaat", "bilirkişi heyeti", "imar durumu", "parsel", "ada", "yeminli"
        },
        ["ICRA_EMRI"] = new List<string> { 
            "icra dairesi", "esas no", "alacaklı", "borçlu", "takip tutarı", "asıl alacak", "faiz", "icra emri", "ödeme emri", "itiraz süresi", "mal beyanı", "haciz", "yediemin"
        },
        ["IFADE_TUTANAGI"] = new List<string> { 
            "ifade tutanağı", "müşteki", "şüpheli", "tanık", "ifade tarihi", "olay yeri", "soruldu", "cevaben", "okundu imzalandı", "kolluk", "sicil no", "avukat"
        },
        ["ARABULUCULUK"] = new List<string> { 
            "arabuluculuk bürosu", "arabulucu", "başvuru no", "taraf", "başvurucu", "karşı taraf", "toplantı tarihi", "anlaşma belgesi", "son tutanak", "imza", "uyuşmazlık türü", "iş hukuku", "ticari"
        },
        ["FERAGAT_DILEKCESI"] = new List<string> { 
            "feragat dilekçesi", "feragat", "davadan", "vazgeçme", "esas no", "mahkemesine", "davacı asil", "kabul", "imza", "tarih", "tebligat"
        },

        // ==========================================
        // 5. LOJİSTİK VE GÜMRÜK BELGELERİ
        // ==========================================
        ["IRSALIYE"] = new List<string> { 
            "sevk irsaliyesi", "irsaliye no", "düzenleme tarihi", "fiili sevk tarihi", "sevk saati", "teslim eden", "teslim alan", "plaka", "şoför tc", "malın cinsi", "miktar", "birim", "açıklama", "fatura irsaliye yerine geçer"
        },
        ["CMR"] = new List<string> { 
            "cmr", "sender", "gönderici", "consignee", "alıcı", "place of delivery", "teslim yeri", "carrier", "taşıyıcı", "truck no", "plaka", "gross weight", "brüt ağırlık", "goods", "mal cinsi"
        },
        ["KONSIMENTO"] = new List<string> { 
            "bill of lading", "konşimento", "shipper", "consignee", "notify party", "vessel", "gemi", "port of loading", "yükleme limanı", "port of discharge", "boşaltma limanı", "container no", "seal no", "mühür no"
        },
        ["GUMRUK_BEYANNAMESI"] = new List<string> { 
            "gümrük beyannamesi", "tescil no", "rejim", "gönderici/ihracatçı", "alıcı/ithalatçı", "beyansahibi", "çıkış ülkesi", "gideceği ülke", "teslim şekli", "gtip", "fatura tutarı", "toplam vergi", "gümrük idaresi"
        },
        ["CEKI_LISTESI"] = new List<string> { 
            "packing list", "çeki listesi", "kap adedi", "net weight", "net ağırlık", "gross weight", "brüt ağırlık", "dimensions", "ölçüler", "description of goods", "mal tanımı", "palet sayısı"
        },
        ["ATR_BELGESI"] = new List<string> { 
            "a.tr", "dolaşım belgesi", "customs endorsement", "exporter", "consignee", "transport details", "remarks", "gross mass", "customs office"
        },
        ["MENSE_SAHADETNAMESI"] = new List<string> { 
            "certificate of origin", "menşe şahadetnamesi", "exporter", "producer", "country of origin", "menşe ülke", "description of goods", "hs code", "gtip"
        },

        // ==========================================
        // 6. KURUMSAL VE İK BELGELERİ
        // ==========================================
        ["IS_SOZLESMESI"] = new List<string> { 
            "işveren", "işçi", "personel", "belirsiz süreli", "belirli süreli", "deneme süresi", "ücret", "net ücret", "brüt ücret", "çalışma saatleri", "görev tanımı", "sözleşme tarihi", "imzalar", "sgk sicil no"
        },
        ["IZIN_FORMU"] = new List<string> { 
            "izin türü", "yıllık izin", "mazeret izni", "ücretsiz izin", "başlangıç tarihi", "bitiş tarihi", "iş başı tarihi", "izin süresi", "izin gün sayısı", "onaylayan", "personel imzası", "vekalet eden"
        },
        ["MASRAF_FORMU"] = new List<string> { 
            "masraf tarihi", "masraf türü", "fiş no", "fatura no", "tutar", "kdv", "toplam masraf", "personel", "açıklama", "onay", "yemek", "yol", "konaklama", "avans"
        },
        ["ZIMMET_TUTANAGI"] = new List<string> { 
            "zimmet tarihi", "malzeme cinsi", "seri no", "marka", "model", "teslim eden", "teslim alan", "demirbaş no", "iade tarihi", "hasarsız"
        },
        ["ISTIFA_DILEKCESI"] = new List<string> { 
            "istifa", "ayrılma talebi", "ihbar süresi", "kendi isteğimle", "hiçbir baskı altında kalmadan", "tarih", "imza", "ad soyad", "departman"
        },
        ["PERFORMANS_FORMU"] = new List<string> { 
            "değerlendirme dönemi", "hedefler", "yetkinlikler", "puan", "yönetici görüşü", "çalışan görüşü", "imza", "tarih", "puanı"
        },
        ["ISG_TUTANAGI"] = new List<string> { 
            "iş sağlığı", "güvenlik", "kkd", "teslim tutanağı", "eğitim tarihi", "eğitime katılanlar", "eğitmen", "imza", "risk değerlendirmesi", "baret", "eldiven"
        },

        // ==========================================
        // 7. RESMİ KAYITLAR VE KİŞİSEL BELGELER
        // ==========================================
        ["KIMLIK_KARTI"] = new List<string> { 
            "t.c. kimlik no", "tr identity no", "soyadı", "surname", "adı", "given name", "doğum tarihi", "date of birth", "seri no", "document no", "geçerlilik tarihi", "cinsiyet", "uyruğu"
        },
        ["EHLIYET"] = new List<string> { 
            "sürücü belgesi", "driving licence", "t.c. kimlik no", "soyadı", "adı", "doğum tarihi", "verildiği tarih", "geçerlilik tarihi", "veren makam", "sınıfı", "kan grubu"
        },
        ["PASAPORT"] = new List<string> { 
            "pasaport no", "passport no", "tip", "type", "ülke kodu", "country code", "soyadı", "surname", "adı", "uyruğu", "nationality", "veren makam", "authority", "doğum yeri"
        },
        ["IKAMETGAH"] = new List<string> { 
            "yerleşim yeri", "diğer adres belgesi", "nüfus müdürlüğü", "tc kimlik no", "ad soyad", "yerleşim yeri adresi", "belgenin veriliş nedeni", "karekod", "barkod", "nüfus kayıt"
        },
        ["NUFUS_KAYIT"] = new List<string> { 
            "vukuatlı nüfus kayıt", "nüfus kayıt örneği", "il", "ilçe", "mahalle", "cilt no", "aile sıra no", "birey sıra no", "anne adı", "baba adı", "medeni hali", "durumu", "sağ", "ölü"
        },
        ["TAPU_SENEDI"] = new List<string> { 
            "tapu senedi", "kat mülkiyeti", "kat irtifakı", "devre mülk", "ili", "ilçe", "bafra", "mahallesi", "ada", "parsel", "yüzölçümü", "nitelik", "arsa payı", "blok no", "bağımsız bölüm", "maliki", "edinme sebebi", "hisse", "yevmiye no"
        },
        ["RUHSAT_ARAC"] = new List<string> { 
            "araç tescil belgesi", "plaka", "markası", "tipi", "ticari adı", "model yılı", "motor no", "şasi no", "sasi no", "kullanım amacı", "cinsi", "rengi", "belge seri no", "tescil tarihi"
        },
        ["SABIKA_KAYDI"] = new List<string> { 
            "adli sicil kaydı", "arşiv kaydı", "yoktur", "vardır", "tc kimlik no", "adı soyadı", "baba adı", "doğum yeri", "sorgulama nedeni", "veriliş tarihi", "resmi kurum", "özel"
        },
        ["VERGI_LEVHASI"] = new List<string> { 
            "vergi levhası", "vergi kimlik no", "tckn", "adı soyadı", "ticaret unvanı", "vergi dairesi", "işe başlama tarihi", "ana faaliyet kodu", "nace kodu", "tasdik eden", "beyan edilen matrah"
        },
        ["IMZA_SIRKULERI"] = new List<string> { 
            "imza sirküleri", "imza beyannamesi", "ticaret sicil memurluğu", "şirket unvanı", "temsil ve ilzama", "münferiden", "müştereken", "noter onay", "yevmiye", "örnek imzalar"
        },
        ["TICARET_SICIL_GAZETESI"] = new List<string> { 
            "ticaret sicili gazetesi", "sayı", "kuruluş", "ana sözleşme", "sermaye", "amaç ve konu", "müdürler", "temsil", "ilan tarihi", "karar tarihi", "mersis numarası"
        },
        ["FAALIYET_BELGESI"] = new List<string> { 
            "faaliyet belgesi", "oda sicil no", "ticaret sicil no", "ticaret odası", "unvanı", "adresi", "meslek grubu", "kayıt tarihi", "sermayesi", "durumu: faal", "işletme adı"
        },

        // ==========================================
        // 8. DİĞER / STANDART ARAMALAR (FALLBACK)
        // ==========================================
        ["DIGER"] = new List<string> { 
            "tarih", "tarihi", "saat", "no", "numara", "toplam", "tutar", "isim", "soyad", "unvan", "tc", "kimlik", "vkn", "vergi no", "tel", "adres", "imza", "kaşe", "açıklama", "sayfa", "durum" 
        }
    };
}
