# 1. SDK Aşaması
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Önce her şeyi kopyala (Hata riskini azaltır)
COPY . .

# Proje dosyasını bul ve restore et
# Eğer klasör içindeyse "dotnet restore Ocr-Pdf/Ocr-Pdf.csproj" yapman gerekebilir 
# Ama alttaki komut klasör fark etmeksizin bulmaya çalışır:
RUN dotnet restore

# Yayınla
RUN dotnet publish -c Release -o /app/publish

# 2. Runtime Aşaması
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# BURASI ÇOK KRİTİK: .dll dosyasının adını tam doğru yazmalısın
# Eğer proje klasör içindeyse "Ocr-Pdf.dll" yerine "bin/Release/.../Ocr-Pdf.dll" 
# gibi bir yerde olabilir ama üstteki publish komutu /app/publish içine toplar.
ENTRYPOINT ["dotnet", "Ocr-Pdf.dll"]
