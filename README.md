# 🥣 Telegram Yemekhane Bot

Bu proje, **her gün saat 11:00'de kurumsal yemekhane menüsünü Telegram kanalına otomatik gönderen bir bot** oluşturmak amacıyla geliştirilmiştir.

## 🚀 Özellikler

✅ Her gün **11:00'de otomatik gönderim**  
✅ **Hafta sonları mesaj göndermez**  
✅ HtmlAgilityPack ile **webden menü çekme**  
✅ `.env` ile **gizli token yönetimi**  
✅ Hata yakalama ve temiz loglama  
✅ .NET Core ile **çapraz platform uyumlu**

---

## ⚙️ Kullanılan Teknolojiler

- [.NET Core](https://dotnet.microsoft.com/)
- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)
- [HtmlAgilityPack](https://html-agility-pack.net/)
- [DotNetEnv](https://www.nuget.org/packages/DotNetEnv)

---

## 🛠️ Kurulum (Windows / Linux)

### 1️ Depoyu Klonla
```bash
git clone https://github.com/kullanici_adin/YemekhaneBot.git
cd YemekhaneBot  
```

--- 

### 2 Bağımlılıkları Yükle
```bash
dotnet restore
```

--- 

### 3️.env Dosyasını Oluştur
```bash
TELEGRAM_BOT_TOKEN=BOT_TOKENINIZ
TELEGRAM_CHANNEL_USER_NAME=@kanal_kullanici_adiniz
```

--- 

## 🖥️ Linux Sunucuda Çalıştırma


### .NET SDK Kurulumu (Ubuntu için)
```bash
sudo apt update
sudo apt install -y dotnet-sdk-8.0 
```

--- 

##### Kurulumu doğrulamak için:
```bash
dotnet --version
```

--- 

### Projeyi Çalıştır
```bash
dotnet run
```
##### ✅ Bot çalışmaya başlayacak ve her gün saat 11:00'de kanala menüyü otomatik gönderecektir.

--- 

## 📞 İletişim
```bash
Proje ile ilgili sorular için @aysenurrcandan üzerinden Telegram’dan iletişime geçebilirsiniz.
```