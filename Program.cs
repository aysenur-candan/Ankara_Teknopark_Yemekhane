using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using DotNetEnv;
class Program
{
    static System.Threading.Timer timer;
    static string telegramToken = "";
    static string kanalKullaniciAdi = "";
    static TelegramBotClient botClient = null;

    static async Task Main()
    {
        Console.WriteLine($"✅ Bot başlatılıyor... {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        string envPath = null;
        string baseDir = AppContext.BaseDirectory;

        for (int i = 0; i < 5; i++)
        {
            string pathToCheck = Path.Combine(baseDir, ".env");
            if (File.Exists(pathToCheck))
            {
                envPath = pathToCheck;
                break;
            }
            baseDir = Directory.GetParent(baseDir)?.FullName;
            if (baseDir == null) break;
        }

        if (envPath == null)
        {
            Console.WriteLine(".env dosyası bulunamadı! Lütfen uygulamanın çalıştığı dizine veya bir üst dizine .env dosyasını yerleştiriniz.");
            return;
        }

        DotNetEnv.Env.Load(envPath);

        telegramToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        if (string.IsNullOrEmpty(telegramToken))
        {
            Console.WriteLine("Telegram bot token bulunamadı! Lütfen .env dosyasını kontrol et.");
            return;
        }

        kanalKullaniciAdi= Environment.GetEnvironmentVariable("TELEGRAM_CHANNEL_USER_NAME");
        if (string.IsNullOrEmpty(kanalKullaniciAdi))
        {
            Console.WriteLine("TELEGRAM_CHANNEL_USER_NAME bulunamadı! Lütfen .env dosyasını kontrol et.");
            return;
        }

        botClient = new TelegramBotClient(telegramToken);

        using var cts = new CancellationTokenSource();

        botClient.StartReceiving(
            new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
            cancellationToken: cts.Token);
        
        TimeSpan hedefSaat = new TimeSpan(11, 0, 0); 
        TimeSpan simdi = DateTime.Now.TimeOfDay;
        TimeSpan ilkCalisma = (simdi < hedefSaat) ? hedefSaat - simdi : hedefSaat.Add(new TimeSpan(24, 0, 0)) - simdi;

        timer = new System.Threading.Timer(async _ =>
        {
            var bugun = DateTime.Today.DayOfWeek;
            if (bugun == DayOfWeek.Saturday || bugun == DayOfWeek.Sunday)
            {
                Console.WriteLine("Bugün hafta sonu, mesaj gönderilmeyecek.");
                return;
            }

            Console.WriteLine("Saat 11:00 - Menü otomatik gönderiliyor...");
            await MenuCekVeKanalaGonder();
        }, null, ilkCalisma, TimeSpan.FromDays(1));

        Console.ReadLine();

    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        return;
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine("📌 Telegram Bot Hatası Yakalandı");

        if (exception is Telegram.Bot.Exceptions.ApiRequestException apiEx)
        {
            Console.WriteLine($"❌ API Hatası: [{apiEx.ErrorCode}] {apiEx.Message}");

            if (!string.IsNullOrEmpty(apiEx.StackTrace))
                Console.WriteLine($"🪐 StackTrace:\n{apiEx.StackTrace}");

            if (apiEx.InnerException != null)
            {
                Console.WriteLine($"🔹 InnerException: {apiEx.InnerException.Message}");
            }
        }
        else
        {
            Console.WriteLine($"❌ Genel Hata: {exception.Message}");

            if (!string.IsNullOrEmpty(exception.StackTrace))
                Console.WriteLine($"🪐 StackTrace:\n{exception.StackTrace}");

            if (exception.InnerException != null)
            {
                Console.WriteLine($"🔹 InnerException: {exception.InnerException.Message}");
            }
        }

        return Task.CompletedTask;
    }


    static async Task<string> GetMenuText()
    {
        var url = "https://www.teknolezzetler.com/yemekhane/";
        using var client = new HttpClient();

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div");

            if (node == null)
                return "Menü bulunamadı.";

            var text = node.InnerText.Trim().Replace("•", "").Replace("\r", "").Replace("\t", "");
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            string tarih = lines[0].Trim();
            var sb = new StringBuilder();
            sb.AppendLine($"📅 TARİH: {tarih}\n");

            var kategoriler = new[] { "ÇORBA", "ANA YEMEK", "YARDIMCI YEMEK", "TATLI", "SALATA VE MEZE" };
            string currentCategory = "";
            int kat_cnt = 0;
            foreach (var lineRaw in lines[1..])
            {
                bool m_control = false;
                if (kat_cnt < kategoriler.Length && lineRaw.Contains(kategoriler[kat_cnt]))
                {
                    if (kat_cnt > 0)
                    {
                        sb.Append("\n🍽️ ");
                    }
                    else
                        sb.Append("🍽️ ");


                    kat_cnt++;

                    m_control=true;
                }
               
                int i = 0;
                foreach (var x in lineRaw)
                {
                 

                    if (x == 10148)
                    {
                        i++;
                        if(i == 1)
                        {
                            if(m_control)
                            {
                                sb.Append("\n- ");
                            }
                            else
                            {
                                sb.Append("- ");
                            }
                        }
                        else
                        {
                            sb.Append("\n- ");
                        }  
                    }
                    else
                    {
                        sb.Append(x);
                    } 
                }             

                sb.Append("\n");
            }

            Console.WriteLine(sb.ToString());

            return sb.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Menü çekme hatası: {ex.Message}");
            return "Menü alınırken bir hata oluştu.";
        }
    }

    static async Task MenuCekVeKanalaGonder()
    {
        Console.WriteLine("Menü çekiliyor...");

        string menuText = await GetMenuText();

        Console.WriteLine("\n📌 Çekilen Menü:\n" + menuText);

        try
        {
            await botClient.SendMessage(
                chatId: kanalKullaniciAdi,
                text: menuText);

            Console.WriteLine($"✅ Menü kanala başarıyla gönderildi. {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Menü kanala gönderilemedi: {ex.Message}");
        }
    }
}
