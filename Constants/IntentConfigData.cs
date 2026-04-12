using System;
using System.Collections.Generic;

namespace ocrProje.Constants;

public static class IntentConfigData
{
    public static readonly Dictionary<string, Dictionary<string, string[]>> IntentConfig = new(StringComparer.OrdinalIgnoreCase)
    {
        // ==========================================
        // 1. İSTİRAHAT VE SAĞLIK RAPORLARI
        // ==========================================
        ["ISTIRAHAT_RAPORU"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["HASTA_AD_SOYAD"] = new[] { "isim", "ad soyad", "hasta adı", "adı soyadı", "kimin", "kime ait" },
            ["HASTA_TCKN"] = new[] { "tc", "kimlik no", "tckn", "tc no", "vatandaşlık no", "tc kimlik" },
            ["HASTA_BABA_ADI"] = new[] { "baba adı", "babasının adı", "babası kim" },
            ["HASTA_DOGUM_YERI"] = new[] { "doğum yeri", "nerede doğmuş", "memleketi", "nereli" },
            ["HASTA_DOGUM_TARIHI"] = new[] { "doğum tarihi", "ne zaman doğmuş", "kaç doğumlu", "yaş" },
            ["HASTA_ADRES"] = new[] { "adres", "nerede oturuyor", "ikametgah", "ev adresi", "il", "ilçe" },
            ["HASTA_TELEFON"] = new[] { "telefon", "cep", "iletişim", "numarası" },
            ["SIGORTALI_TURU"] = new[] { "sigortalı türü", "4a", "4b", "4c", "bağkur", "emekli sandığı" },
            ["SGK_SICIL_NO"] = new[] { "sgk sicil", "sigorta sicil no", "sigorta numarası" },
            ["HASTANE_ADI"] = new[] { "hastane", "sağlık tesisi", "kurum adı", "nereden alınmış", "sağlık bakanlığı", "aile hekimliği" },
            ["POLIKLINIK_SERVIS"] = new[] { "poliklinik", "servis", "hangi bölüm", "klinik", "branş" },
            ["IL_ILCE_BILGISI"] = new[] { "il", "ilçe", "hangi şehir", "neresi vermiş" },
            ["HEKIM_AD_SOYAD"] = new[] { "doktor", "hekim", "muayene eden", "yazan doktor", "imzalayan", "doktor adı" },
            ["HEKIM_BRANS"] = new[] { "hekim branşı", "doktor uzmanlığı", "uzmanlık alanı", "branş adı", "pratisyen", "uzman" },
            ["HEKIM_BRANS_KODU"] = new[] { "branş kodu", "uzmanlık kodu" },
            ["HEKIM_DIPLOMA_NO"] = new[] { "diploma no", "tescil no", "doktor sicil", "diploma numarası" },
            ["RAPOR_TAKIP_NO"] = new[] { "takip no", "sgk rapor takip", "rapor takip no", "sorgulama no" },
            ["PROTOKOL_NO"] = new[] { "protokol", "protokol no", "işlem numarası", "kayıt no" },
            ["VAKA_TURU"] = new[] { "vaka", "vaka türü", "iş kazası", "meslek hastalığı", "hastalık", "analık", "iş kazası mı" },
            ["TANI_ICD_KODU"] = new[] { "icd", "icd kodu", "hastalık kodu", "tanı kodu" },
            ["TANI_ACIKLAMA"] = new[] { "tanı", "teşhis", "neyi var", "hastalığı ne", "neden rapor almış", "tanılar" },
            ["RAPOR_DURUMU"] = new[] { "rapor durumu", "çalışır", "kontrol", "durumu ne", "sonuç" },
            ["YATIS_TARIHI"] = new[] { "yatış tarihi", "hastaneye yatış", "ne zaman yattı", "yatış" },
            ["CIKIS_TARIHI"] = new[] { "çıkış tarihi", "hastaneden çıkış", "taburcu tarihi", "ne zaman çıktı" },
            ["AYAKTA_BASLAMA_TARIHI"] = new[] { "ayakta başlama", "rapor başlangıç", "ne zaman başladı", "başlama tarihi" },
            ["AYAKTA_BITIS_TARIHI"] = new[] { "ayakta bitiş", "rapor bitiş", "ne zaman bitiyor", "bitiş tarihi" },
            ["ISBASI_KONTROL_TARIHI"] = new[] { "işbaşı tarihi", "kontrol tarihi", "ne zaman işe başlayacak", "işbaşı", "kontrole ne zaman" },
            ["RAPOR_SURESI_GUN"] = new[] { "kaç gün", "rapor süresi", "gün sayısı", "kaç günlük" },
            ["BELGE_TARIHI"] = new[] { "belge tarihi", "düzenlenme tarihi", "ne zaman yazılmış", "raporun yazıldığı tarih" },
            ["BASTABIP_ONAYI"] = new[] { "başhekim", "baştabip", "onaylayan", "onay makamı" },
            ["BARKOD_KAREKOD"] = new[] { "barkod", "karekod", "doğrulama kodu", "karekod no", "referans kodu" }
        },

        // ==========================================
        // 2. BANKA DEKONTLARI (EFT/HAVALE/SWIFT)
        // ==========================================
        ["DEKONT"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["BANKA_ADI"] = new[] { "bankası", "banka", "hangi banka", "kurum" },
            ["SUBE_ADI"] = new[] { "şubesi", "şube adı", "hangi şube" },
            ["SUBE_KODU"] = new[] { "şube kodu", "şube no", "kod" },
            ["GONDEREN_AD_SOYAD"] = new[] { "gönderen", "gönderen adı", "kim yollamış", "gönderici", "ismi" },
            ["GONDEREN_UNVAN"] = new[] { "gönderen unvan", "firma", "şirket" },
            ["GONDEREN_VKN_TCKN"] = new[] { "gönderen tc", "gönderen vkn", "tc kimlik", "vergi no" },
            ["GONDEREN_IBAN"] = new[] { "gönderen iban", "hangi ibandan", "çıkış yapılan iban" },
            ["GONDEREN_HESAP_NO"] = new[] { "gönderen hesap", "hesap no", "hesap numarası" },
            ["GONDEREN_MUSTERI_NO"] = new[] { "müşteri no", "müşteri numarası" },
            ["ALICI_AD_SOYAD"] = new[] { "alıcı", "alıcı adı", "kime gitmiş", "alici" },
            ["ALICI_UNVAN"] = new[] { "alıcı unvan", "alıcı firma", "karşı taraf" },
            ["ALICI_VKN_TCKN"] = new[] { "alıcı tc", "alıcı vkn", "alıcı vergi no" },
            ["ALICI_IBAN"] = new[] { "alıcı iban", "hangi ibana gitmiş", "iban" },
            ["ALICI_HESAP_NO"] = new[] { "alıcı hesap", "karşı hesap no" },
            ["ISLEM_TARIHI"] = new[] { "işlem tarihi", "tarih", "ne zaman", "dekont tarihi" },
            ["ISLEM_SAATI"] = new[] { "işlem saati", "saat", "ne zaman gönderilmiş" },
            ["VALOR_TARIHI"] = new[] { "valör", "valör tarihi", "hesaba geçiş" },
            ["DEKONT_NO"] = new[] { "dekont no", "dekont numarası", "işlem no", "sorgu no" },
            ["REFERANS_NO"] = new[] { "referans no", "referans numarası", "ref no" },
            ["ISLEM_TURU"] = new[] { "işlem türü", "havale mi eft mi", "eft", "havale", "virman", "swift" },
            ["KANAL_BILGISI"] = new[] { "kanal", "internet", "mobil", "şube" },
            ["ISLEM_TUTARI"] = new[] { "işlem tutarı", "tutar", "meblağ", "kaç para", "ne kadar" },
            ["DOVIZ_CINSI"] = new[] { "döviz cinsi", "para birimi", "tl", "usd", "eur" },
            ["KOMISYON_TUTARI"] = new[] { "komisyon", "masraf", "işlem ücreti", "kesinti" },
            ["BSMV_TUTARI"] = new[] { "bsmv", "vergi", "vergi tutarı" },
            ["TOPLAM_TAHSILAT"] = new[] { "toplam tahsilat", "toplam tutar", "toplam kesilen" },
            ["BAKIYE"] = new[] { "bakiye", "kalan bakiye", "hesap bakiyesi" },
            ["ACIKLAMA"] = new[] { "açıklama", "işlem açıklaması", "not", "ne için" }
        },

        // ==========================================
        // 3. E-FATURA VE ARŞİV FATURALAR
        // ==========================================
        ["FATURA"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["FATURA_NO"] = new[] { "fatura no", "belge no", "fatura numarası", "seri no", "fatura id" },
            ["FATURA_TARIHI"] = new[] { "fatura tarihi", "düzenleme tarihi", "ne zaman kesilmiş", "tarih" },
            ["FATURA_SAATI"] = new[] { "fatura saati", "düzenleme saati", "saat" },
            ["ETTN"] = new[] { "ettn", "evrensel tekil", "ettn no" },
            ["SENARYO"] = new[] { "senaryo", "temel", "ticari", "e-arşiv" },
            ["FATURA_TIPI"] = new[] { "fatura tipi", "satış", "iade", "tevkifat", "istisna" },
            ["IRSALIYE_NO"] = new[] { "irsaliye no", "irsaliye numarası" },
            ["IRSALIYE_TARIHI"] = new[] { "irsaliye tarihi", "sevk tarihi" },
            ["SATICI_UNVAN"] = new[] { "satıcı", "kim kesmiş", "gönderici firma", "unvan" },
            ["SATICI_VKN_TCKN"] = new[] { "satıcı vkn", "satıcı tc", "vergi no", "vergi numarası", "vkn" },
            ["SATICI_VD"] = new[] { "satıcı vergi dairesi", "vergi dairesi", "vd", "v.d." },
            ["SATICI_MERSIS"] = new[] { "mersis", "mersis no", "ticaret sicil" },
            ["SATICI_ADRES"] = new[] { "satıcı adresi", "adres", "merkez", "şube adresi" },
            ["SATICI_IBAN"] = new[] { "satıcı iban", "ödeme iban", "hesap no" },
            ["ALICI_UNVAN"] = new[] { "alıcı", "kime kesilmiş", "sayın", "müşteri", "müşteri unvanı" },
            ["ALICI_VKN_TCKN"] = new[] { "alıcı vkn", "alıcı tc", "müşteri vkn", "tckn" },
            ["ALICI_VD"] = new[] { "alıcı vergi dairesi", "müşteri vd" },
            ["ALICI_ADRES"] = new[] { "alıcı adres", "teslimat adresi", "fatura adresi" },
            ["MAL_HIZMET_TOPLAMI"] = new[] { "mal hizmet toplamı", "ara toplam", "kdv hariç", "matrah" },
            ["TOPLAM_ISKONTO"] = new[] { "iskonto", "indirim", "indirim tutarı" },
            ["HESAPLANAN_KDV"] = new[] { "hesaplanan kdv", "kdv tutarı", "toplam kdv", "ne kadar kdv" },
            ["KDV_ORANI"] = new[] { "kdv oranı", "%20", "%18", "%10", "%1", "kdv yüzdesi" },
            ["VERGI_DAHIL_TOPLAM"] = new[] { "vergi dahil", "kdv dahil toplam", "brüt" },
            ["TEVKIFAT_ORANI"] = new[] { "tevkifat oranı", "tevkifat", "kaç bölü kaç" },
            ["TEVKIFAT_TUTARI"] = new[] { "tevkifat tutarı", "kesilen kdv" },
            ["ODENECEK_TUTAR"] = new[] { "ödenecek tutar", "genel toplam", "toplam", "yekün", "ödenecek" },
            ["YAZIYLA_TUTAR"] = new[] { "yazıyla", "yazı ile", "yalnız" },
            ["URUN_HIZMET_ADI"] = new[] { "ürün adı", "hizmet", "ne alınmış", "malın cinsi", "açıklama" },
            ["MIKTAR"] = new[] { "miktar", "adet", "kg", "litre" },
            ["BIRIM_FIYAT"] = new[] { "birim fiyat", "tanesi ne kadar", "fiyat" },
            ["ODEME_SEKLI"] = new[] { "ödeme şekli", "nakit", "kredi kartı", "vadeli" },
            ["NOTLAR_ACIKLAMA"] = new[] { "not", "açıklama", "fatura notu" }
        },

        // ==========================================
        // 4. PERSONEL MAAŞ BORDROSU
        // ==========================================
        ["BORDRO"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["ISYERI_UNVAN"] = new[] { "işyeri", "firma", "şirket", "nerede çalışıyor" },
            ["SGK_ISYERI_NO"] = new[] { "işyeri sicil", "sgk işyeri no", "işyeri sicil no" },
            ["PERSONEL_AD_SOYAD"] = new[] { "isim", "ad soyad", "personel", "kimin bordrosu", "çalışan" },
            ["PERSONEL_TCKN"] = new[] { "tc kimlik", "tckn", "personel tc", "kimlik no" },
            ["PERSONEL_SGK_NO"] = new[] { "sgk no", "sigorta numarası", "sgk sicil no" },
            ["PERSONEL_SICIL_NO"] = new[] { "sicil no", "personel no", "yaka no" },
            ["DEPARTMAN_GOREV"] = new[] { "departman", "bölüm", "unvan", "görev", "pozisyon" },
            ["BORDRO_DONEMI_AY"] = new[] { "ay", "hangi ay", "dönem ay" },
            ["BORDRO_DONEMI_YIL"] = new[] { "yıl", "dönem yıl", "hangi yıl" },
            ["ISE_GIRIS_TARIHI"] = new[] { "işe giriş", "giriş tarihi", "ne zaman başlamış" },
            ["CIKIS_TARIHI"] = new[] { "çıkış tarihi", "işten ayrılma", "ayrılış tarihi" },
            ["CALISILAN_GUN"] = new[] { "çalışılan gün", "normal gün", "kaç gün çalışmış" },
            ["HAFTA_TATILI"] = new[] { "hafta tatili", "pazar", "tatil günü" },
            ["RAPOR_GUNU"] = new[] { "rapor günü", "istirahat günü", "sıhhi izin" },
            ["SGK_GUNU"] = new[] { "sgk günü", "prim günü", "toplam gün" },
            ["BRUT_UCRET"] = new[] { "brüt ücret", "brüt maaş", "sözleşme ücreti" },
            ["AYLIK_NORMAL_KAZANC"] = new[] { "normal kazanç", "aylık kazanç", "hak ediş" },
            ["FAZLA_MESAI_SAATI"] = new[] { "mesai saati", "fazla çalışma saat" },
            ["FAZLA_MESAI_UCRETI"] = new[] { "mesai ücreti", "fazla mesai", "mesai" },
            ["IKRAMIYE_PRIM"] = new[] { "ikramiye", "prim", "ödül", "sosyal yardım" },
            ["TOPLAM_KAZANC"] = new[] { "toplam kazanç", "brüt kazanç toplamı", "kazançlar" },
            ["SGK_ISCI_PAYI"] = new[] { "sgk işçi payı", "sigorta kesintisi", "sgk primi" },
            ["ISSIZLIK_ISCI_PAYI"] = new[] { "işsizlik", "işsizlik kesintisi", "işsizlik primi" },
            ["GELIR_VERGISI_MATRAHI"] = new[] { "gelir vergisi matrahı", "vergi matrahı" },
            ["KUMULATIF_VERGI_MATRAHI"] = new[] { "kümülatif", "kümülatif matrah", "toplam matrah" },
            ["GELIR_VERGISI"] = new[] { "gelir vergisi", "gv kesintisi" },
            ["DAMGA_VERGISI"] = new[] { "damga vergisi", "dv kesintisi" },
            ["BES_KESINTISI"] = new[] { "bes", "bireysel emeklilik", "emeklilik kesintisi" },
            ["TOPLAM_KESINTI"] = new[] { "toplam kesinti", "kesintiler", "kesinti yekün" },
            ["NET_ODENEN_UCRET"] = new[] { "net ücret", "net maaş", "ele geçen", "ödenecek net", "net ödenen" }
        },

        // ==========================================
        // 5. NOTER EVRAKLARI (Vekalet, İhtarname vb.)
        // ==========================================
        ["NOTER"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["NOTERLIK_ADI"] = new[] { "noterliği", "hangi noter", "noterlik" },
            ["NOTER_AD_SOYAD"] = new[] { "noter", "noter adı", "tasdik eden" },
            ["NOTER_BAS_KATIP"] = new[] { "baş katip", "katip", "vekili" },
            ["YEVMIYE_NO"] = new[] { "yevmiye no", "yevmiye", "işlem numarası", "yev. no" },
            ["ISLEM_TARIHI"] = new[] { "tarih", "düzenleme tarihi", "onay tarihi" },
            ["ISLEM_SAATI"] = new[] { "saat", "işlem saati", "onay saati" },
            ["BELGE_TURU"] = new[] { "vekaletname", "ihtarname", "azilname", "muvafakatname", "ne belgesi" },
            ["VEKIL_EDEN_AD_SOYAD"] = new[] { "vekil eden", "vekalet veren", "ihtar eden", "keşideci" },
            ["VEKIL_EDEN_TCKN_VKN"] = new[] { "vekil eden tc", "tc kimlik", "vkn", "kimlik no" },
            ["VEKIL_EDEN_BABA_ADI"] = new[] { "vekil eden baba adı", "baba adı" },
            ["VEKIL_EDEN_DOGUM_TARIHI"] = new[] { "doğum tarihi", "doğum yılı" },
            ["VEKIL_EDEN_ADRES"] = new[] { "vekil eden adres", "adresi", "ikametgah" },
            ["VEKIL_AD_SOYAD"] = new[] { "vekil", "muhatap", "kime vermiş", "temsilci" },
            ["VEKIL_TCKN_VKN"] = new[] { "vekil tc", "muhatap tc", "vekil vkn" },
            ["VEKIL_BABA_ADI"] = new[] { "vekil baba adı", "muhatap baba adı" },
            ["VEKIL_ADRES"] = new[] { "vekil adresi", "muhatap adresi" },
            ["MAKBUZ_NO"] = new[] { "makbuz no", "makbuz", "tahsilat seri" },
            ["HARC_MATRAHI"] = new[] { "harç matrahı", "matrah" },
            ["NOTER_UCRETI"] = new[] { "noter ücreti", "ücreti" },
            ["YAZI_UCRETI"] = new[] { "yazı ücreti", "yazım masrafı" },
            ["CEVIRMEN_UCRETI"] = new[] { "çevirmen ücreti", "tercüman masrafı" },
            ["DAMGA_VERGISI"] = new[] { "damga vergisi", "damga" },
            ["DEGERLI_KAGIT_BEDELI"] = new[] { "değerli kağıt", "kağıt bedeli" },
            ["KDV_TUTARI"] = new[] { "kdv", "hesaplanan kdv" },
            ["TOPLAM_MAKBUZ_TUTARI"] = new[] { "toplam tutar", "ne kadar ödenmiş", "yekün", "tahsil edilen" },
            ["ARAC_PLAKA"] = new[] { "plaka", "araç plakası", "hangi araç" },
            ["ARAC_SASI_NO"] = new[] { "şasi no", "şasi numarası", "motor no" },
            ["ARAC_BEDELI"] = new[] { "araç bedeli", "satış bedeli", "kasko değeri" },
            ["TERCUMAN_AD_SOYAD"] = new[] { "tercüman", "çevirmen", "yeminli tercüman" },
            ["DAYANAK_BELGE"] = new[] { "dayanak", "ilgi", "hangi belgeye istinaden" }
        },

        // ==========================================
        // 7. TAPU SENEDİ (Gayrimenkul ve Mülkiyet)
        // ==========================================
        ["TAPU_SENEDI"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["ILI"] = new[] { "ili", "il", "hangi il", "şehir", "şehrinde", "ilinde" },
            ["ILCESI"] = new[] { "ilçesi", "ilçe", "hangi ilçe", "bafra", "kadıköy", "ilçesinde" },
            ["MAHALLESI"] = new[] { "mahallesi", "mahalle", "mah.", "hangi mahalle" },
            ["KOYU"] = new[] { "köyü", "köy", "hangi köy", "ky." },
            ["SOKAGI"] = new[] { "sokağı", "sokak", "sok.", "cadde", "bulvar" },
            ["MEVKII"] = new[] { "mevkii", "mevki", "hangi mevki", "bölge" },
            ["PAFTA_NO"] = new[] { "pafta", "pafta no", "paftası", "pafta numarası" },
            ["ADA_NO"] = new[] { "ada", "ada no", "adası", "ada numarası" },
            ["PARSEL_NO"] = new[] { "parsel", "parsel no", "parseli", "parsel numarası" },
            ["YUZOLCUMU"] = new[] { "yüzölçümü", "yüz ölçümü", "metrekaresi", "m2", "alanı", "büyüklüğü", "metrekare" },
            ["NITELIGI"] = new[] { "niteliği", "nitelik", "cinsi", "türü", "arsa", "tarla", "mesken", "kargir ev" },
            ["ZEMIN_TIPI"] = new[] { "zemin tipi", "zemin türü", "ana taşınmaz", "kat mülkiyeti zemin" },
            ["KAT_MULKIYETI"] = new[] { "kat mülkiyeti", "kat irtifakı", "devre mülk", "mülkiyet tipi" },
            ["ARSA_PAYI"] = new[] { "arsa payı", "arsadaki pay", "pay", "oran" },
            ["BLOK_NO"] = new[] { "blok", "blok no", "blok numarası", "hangi blok" },
            ["KAT_NO"] = new[] { "kat", "kat no", "katı", "kaçıncı kat" },
            ["BAGIMSIZ_BOLUM_NO"] = new[] { "bağımsız bölüm", "bağımsız bölüm no", "bb no", "daire no", "kapı no", "dükkan no" },
            ["EDINME_SEBEBI"] = new[] { "edinme sebebi", "iktisap sebebi", "nasıl alınmış", "satış", "miras", "intikal", "bağış" },
            ["MALIK_AD_SOYAD"] = new[] { "maliki", "sahibi", "kimin üzerine", "tapu sahibi", "adı soyadı", "isim", "malik" },
            ["MALIK_TCKN"] = new[] { "tc kimlik", "tckn", "malik tc", "kimlik numarası", "vkn", "vergi no" },
            ["BABA_ADI"] = new[] { "baba adı", "babasının adı", "babası" },
            ["HISSE_PAYI"] = new[] { "hisse", "hissesi", "hisse payı", "ne kadar hisse", "pay", "tam" },
            ["HISSE_PAYDASI"] = new[] { "hisse paydası", "payda" },
            ["YEVMIYE_NO"] = new[] { "yevmiye no", "yevmiye", "işlem numarası", "yev. no" },
            ["CILT_NO"] = new[] { "cilt no", "cilt", "defter no" },
            ["SAHIFE_NO"] = new[] { "sahife no", "sayfa", "sahife", "sayfa no" },
            ["TESCIL_TARIHI"] = new[] { "tescil tarihi", "alım tarihi", "ne zaman alınmış", "tarih" },
            ["SERH_BEYAN"] = new[] { "şerh", "beyan", "irtifak", "kısıtlama", "şerhler" },
            ["IPOTEK_DURUMU"] = new[] { "ipotek", "rehin", "haciz", "ipotekli mi" },
            ["TAPU_MUDURLUGU"] = new[] { "tapu sicil müdürlüğü", "tapu müdürlüğü", "hangi müdürlük", "veren makam" },
            ["BARKOD_NO"] = new[] { "barkod", "zemin no", "karekod", "e-devlet doğrulama" }
        },

        // ==========================================
        // 8. MAHKEME KARARI VE HUKUKİ EVRAKLAR
        // ==========================================
        ["MAHKEME_KARARI"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["MAHKEME_ADI"] = new[] { "mahkemesi", "mahkeme", "hangi mahkeme", "asliye hukuk", "sulh hukuk", "ağır ceza", "t.c." },
            ["ESAS_NO"] = new[] { "esas no", "esas", "dosya no", "esas numarası", "dosya numarası" },
            ["KARAR_NO"] = new[] { "karar no", "karar numarası", "karar" },
            ["C_SAVCILIGI_NO"] = new[] { "soruşturma no", "c. savcılığı", "savcılık esas", "iddianame no" },
            ["DAVACI_ASIL"] = new[] { "davacı", "davacı asil", "şikayetçi", "müşteki", "katılan", "dava eden" },
            ["DAVALI"] = new[] { "davalı", "sanık", "şüpheli", "karşı taraf", "kendisine dava açılan" },
            ["DAVACI_VEKILI"] = new[] { "davacı vekili", "avukatı", "davacı avukatı", "vekil" },
            ["DAVALI_VEKILI"] = new[] { "davalı vekili", "sanık müdafii", "savunan avukat" },
            ["DAVA_TURU"] = new[] { "dava", "dava türü", "suç", "konusu", "davanın konusu", "aidiyetinin tespiti", "muhdesat", "boşanma", "tazminat", "miras" },
            ["DAVA_TARIHI"] = new[] { "dava tarihi", "açılış tarihi", "ne zaman açılmış" },
            ["DURUSMA_TARIHI"] = new[] { "duruşma tarihi", "celse tarihi", "mahkeme günü" },
            ["KARAR_TARIHI"] = new[] { "karar tarihi", "hüküm tarihi", "ne zaman bitti", "sonuçlandığı tarih" },
            ["GEREKCELI_KARAR_TARIHI"] = new[] { "gerekçeli karar tarihi", "yazım tarihi", "kararın yazıldığı" },
            ["HAKIM_SICIL"] = new[] { "hakim", "başkan", "üye", "sicil", "hakim sicil" },
            ["KATIP_SICIL"] = new[] { "katip", "zabıt katibi", "yazman" },
            ["KARAR_METNI_HUKUM"] = new[] { "hüküm", "karar verildi", "sonuç", "hükmedildi", "türk milleti adına", "cezalandırılmasına", "kabulüne", "reddine" },
            ["GEREKCE"] = new[] { "gerekçe", "delillerin değerlendirilmesi", "neden", "sebebi" },
            ["HARC_TUTARI"] = new[] { "harç", "peşin harç", "karar ilam harcı", "ne kadar harç" },
            ["VEKALET_UCRETI"] = new[] { "vekalet ücreti", "avukatlık ücreti", "karşı taraf vekalet" },
            ["YARGILAMA_GIDERI"] = new[] { "yargılama gideri", "masraf", "posta gideri", "bilirkişi ücreti" },
            ["KESIF_TARIHI"] = new[] { "keşif", "keşif tarihi", "yerinde inceleme" },
            ["BILIRKISI_RAPOR_TARIHI"] = new[] { "bilirkişi", "bilirkişi raporu", "rapor tarihi", "uzman raporu" },
            ["FERAGAT_BEYANI"] = new[] { "feragat", "vazgeçme", "feragat dilekçesi", "davadan vazgeçti" },
            ["PARSEL_BILGISI"] = new[] { "parsel", "taşınmaz", "dava konusu yer" },
            ["ADA_BILGISI"] = new[] { "ada", "pafta" },
            ["TEBLIGAT_ADRESI"] = new[] { "tebligat", "adres", "mermis adresi", "tebliğ edilecek yer" },
            ["KESINLESME_DURUMU"] = new[] { "kesinleşme", "kesinleşti", "şerhi", "itiraz edilmedi" },
            ["TARAFLARIN_IDDIASI"] = new[] { "iddia", "davacı iddiaları", "talep", "istek" },
            ["DELILLER"] = new[] { "deliller", "kanıtlar", "tanık beyanları", "kamera kayıtları" },
            ["KANUN_YOLU_ISTINAF"] = new[] { "istinaf", "temyiz", "yargıtay", "itiraz süresi", "bölge adliye" }
        },

        // ==========================================
        // 9. KREDİ KARTI EKSTRESİ
        // ==========================================
        ["KREDI_KARTI_EKSTRESI"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["MUSTERI_AD_SOYAD"] = new[] { "sayın", "isim", "ad soyad", "müşteri", "kart sahibi" },
            ["MUSTERI_NO"] = new[] { "müşteri no", "müşteri numarası", "hesap no" },
            ["TCKN_VKN"] = new[] { "tc kimlik", "tckn", "vkn", "vergi no" },
            ["KART_NO"] = new[] { "kart no", "kart numarası", "kredi kartı no", "son 4 hane", "kartınız" },
            ["KART_TIPI"] = new[] { "kart tipi", "visa", "mastercard", "troy", "gold", "platinum" },
            ["EKSTRE_DONEMI"] = new[] { "dönem", "ekstre dönemi", "hangi ay", "hesap dönemi" },
            ["HESAP_KESIM_TARIHI"] = new[] { "hesap kesim tarihi", "kesim tarihi", "ekstre tarihi" },
            ["SON_ODEME_TARIHI"] = new[] { "son ödeme", "son ödeme tarihi", "vade", "ne zaman ödenecek" },
            ["TOPLAM_BORC"] = new[] { "dönem borcu", "toplam borç", "borcunuz", "ödenmesi gereken" },
            ["ASGARI_ODEME"] = new[] { "asgari", "asgari ödeme", "asgari tutar", "minimum ödeme" },
            ["ONCEKI_BAKIYE"] = new[] { "önceki bakiye", "devreden", "geçen aydan kalan", "önceki ay" },
            ["ODENEN_TUTAR"] = new[] { "ödenen", "tahsilat", "yatırılan tutar", "ödeme" },
            ["DONEM_ICI_HARCAMALAR"] = new[] { "harcamalar", "dönem içi harcama", "alışverişler", "bu ayki harcamalar" },
            ["KREDI_LIMITI"] = new[] { "limit", "kredi limiti", "toplam limit", "kart limiti" },
            ["KULLANILABILIR_LIMIT"] = new[] { "kullanılabilir", "kalan limit", "kullanılabilir limit", "ne kadar kaldı" },
            ["NAKIT_AVANS_LIMITI"] = new[] { "nakit avans limiti", "avans limiti" },
            ["KULLANILABILIR_NAKIT_AVANS"] = new[] { "kalan nakit avans", "kullanılabilir avans", "çekilebilir para" },
            ["ALISVERIS_FAIZI"] = new[] { "akdi faiz", "alışveriş faizi", "faiz oranı", "aylık faiz" },
            ["GECIKME_FAIZI"] = new[] { "gecikme faizi", "temerrüt faizi", "ceza faizi" },
            ["BSMV_KKDF"] = new[] { "bsmv", "kkdf", "vergiler" },
            ["KAZANILAN_PUAN"] = new[] { "kazanılan", "bu ay kazanılan puan", "bonus", "maxipuan", "worldpuan" },
            ["KULLANILAN_PUAN"] = new[] { "kullanılan puan", "harcanan puan" },
            ["TOPLAM_PUAN"] = new[] { "toplam puan", "kullanılabilir puan", "puan bakiyesi", "biriken" },
            ["HARCAMA_TARIHI"] = new[] { "işlem tarihi", "harcama tarihi", "ne zaman alınmış" },
            ["ISLEM_TUTARI"] = new[] { "işlem tutarı", "harcama tutarı", "tutar" },
            ["ISLEM_ACIKLAMASI"] = new[] { "işlem açıklaması", "nereden alınmış", "firma adı", "mağaza" },
            ["PROVIZYON_NO"] = new[] { "provizyon", "prov kodu", "referans no" },
            ["SEKTOR_BILGISI"] = new[] { "sektör", "giyim", "market", "akaryakıt", "kategori" },
            ["IBAN_ODEME"] = new[] { "iban", "tr", "ödeme kanalı", "hesap numarası" },
            ["MUSTERI_ADRES"] = new[] { "adres", "ikametgah", "iletişim adresi" }
        },

        // ==========================================
        // 10. KONŞİMENTO / CMR (Gümrük ve Lojistik)
        // ==========================================
        ["KONSIMENTO"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["SHIPPER_GONDERICI"] = new[] { "shipper", "gönderici", "ihracatçı", "exporter", "sender", "kim gönderiyor" },
            ["CONSIGNEE_ALICI"] = new[] { "consignee", "alıcı", "ithalatçı", "importer", "receiver", "kime gidiyor" },
            ["NOTIFY_PARTY"] = new[] { "notify", "notify party", "haber verilecek", "bildirim tarafı" },
            ["VESSEL_GEMI_ADI"] = new[] { "vessel", "gemi", "gemi adı", "taşıyıcı isim", "ocean vessel" },
            ["VOYAGE_SEFER_NO"] = new[] { "voyage", "sefer", "sefer no", "voyage no" },
            ["PORT_OF_LOADING"] = new[] { "port of loading", "pol", "yükleme limanı", "nereden yüklendi", "loading port" },
            ["PORT_OF_DISCHARGE"] = new[] { "port of discharge", "pod", "boşaltma limanı", "tahliye limanı" },
            ["FINAL_DESTINATION"] = new[] { "final destination", "son varış", "teslim yeri", "place of delivery" },
            ["CONTAINER_NO"] = new[] { "container no", "konteyner", "konteyner no", "equipment no" },
            ["SEAL_MUHUR_NO"] = new[] { "seal no", "mühür", "mühür no" },
            ["GROSS_WEIGHT"] = new[] { "gross weight", "brüt", "brüt ağırlık", "kgr", "kgs", "total weight" },
            ["NET_WEIGHT"] = new[] { "net weight", "net ağırlık", "darası düşülmüş" },
            ["MEASUREMENT_HACIM"] = new[] { "measurement", "volume", "hacim", "cbm", "m3" },
            ["DESCRIPTION_OF_GOODS"] = new[] { "description", "mal cinsi", "ürün tanımı", "goods", "ne taşıyor", "eşya" },
            ["HS_CODE_GTIP"] = new[] { "hs code", "gtip", "tarife", "gümrük kodu" },
            ["FREIGHT_TERMS"] = new[] { "freight", "navlun", "prepaid", "collect", "freight terms", "ödeme şekli" },
            ["DATE_OF_ISSUE"] = new[] { "date of issue", "düzenleme tarihi", "tarih", "issued date" },
            ["BL_NUMBER"] = new[] { "b/l no", "bill of lading no", "konşimento no", "belge numarası" },
            ["PLACE_OF_RECEIPT"] = new[] { "place of receipt", "teslim alınma yeri" },
            ["NO_OF_PACKAGES"] = new[] { "packages", "kap adedi", "kaç kap", "adet", "quantity", "pieces" },
            ["PACKAGE_TYPE"] = new[] { "package type", "palet", "kutu", "çuval", "karton", "ambalaj şekli" },
            ["MARKS_AND_NUMBERS"] = new[] { "marks and numbers", "marka ve no", "işaretler" },
            ["ON_BOARD_DATE"] = new[] { "shipped on board", "on board date", "yükleme tarihi" },
            ["CARRIER_NAME"] = new[] { "carrier", "taşıyıcı", "armatör", "lojistik firması", "nakliyeci" },
            ["AGENT_DETAILS"] = new[] { "agent", "acente", "teslim acentesi", "delivery agent" },
            ["FREIGHT_PAYABLE_AT"] = new[] { "payable at", "ödeme yeri", "navlun nerede ödenecek" },
            ["ORIGINAL_BL_COUNT"] = new[] { "no of original", "orijinal adedi", "kaç nüsha" },
            ["SHIPPERS_REFERENCE"] = new[] { "shipper's ref", "gönderici referansı", "fatura no" },
            ["CONSIGNEES_REFERENCE"] = new[] { "consignee's ref", "alıcı referansı", "sipariş no", "po no" },
            ["PLACE_OF_ISSUE"] = new[] { "place of issue", "düzenlendiği yer", "nerede basıldı" }
        },

        // ==========================================
        // DİĞER (BİLİNMEYEN BELGELER) İÇİN FALLBACK
        // ==========================================
        ["DIGER"] = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["AD_SOYAD"]   = new[] { "isim", "ad soyad", "ad", "soyad", "kimin", "kişi", "adı", "ismi", "hasta", "hekim", "doktor" },
            ["TUTAR"]      = new[] { "tutar", "fiyat", "para", "ne kadar", "ücret", "meblağ", "toplam", "yekün", "ödenecek", "bedel" },
            ["TARIH"]      = new[] { "tarih", "ne zaman", "gün", "saat", "vakit", "hangi gün", "date", "zaman" },
            ["TCKN"]       = new[] { "tc", "kimlik", "no", "numara", "tckn", "vatandaş", "tc kimlik", "nüfus no" },
            ["VKN"]        = new[] { "vergi no", "vkn", "vergi numarası", "şirket no", "firma no", "ticari numara" },
            ["IBAN"]       = new[] { "iban", "hesap", "hesap no", "tr", "banka", "hesap listesi" },
            ["ADRES"]      = new[] { "adres", "yer", "nerede", "il", "ilçe", "mahalle", "adres bilgisi", "konum" },
            ["TELEFON"]    = new[] { "telefon", "tel", "cep", "numara", "gsm", "phone" }
        }
    };
}
