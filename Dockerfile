# 1. SDK Aşaması (Build için)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve restore et
# Eğer projen bir alt klasördeyse yolu "Ocr-Pdf/Ocr-Pdf.csproj" şeklinde düzelt
COPY ["Ocr-Pdf.csproj", "./"]
RUN dotnet restore "./Ocr-Pdf.csproj"

# Tüm dosyaları kopyala ve yayınla
COPY . .
RUN dotnet publish "Ocr-Pdf.csproj" -c Release -o /app/publish

# 2. Runtime Aşaması (Çalıştırma için)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render için port ve çevre değişkenleri
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Uygulamayı başlat
# Ocr-Pdf.dll kısmının proje adınla aynı olduğundan emin ol
ENTRYPOINT ["dotnet", "Ocr-Pdf.dll"]
