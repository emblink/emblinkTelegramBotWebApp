using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using BarracudaTestBot.Checkers;

namespace BarracudaTestBot
{
    public static class Utilities
    {
        public static async Task<Message> SendText(this ITelegramBotClient botClient,
            int? replyTo,
            long chatId,
            string text,
            CancellationToken cancellationToken, ParseMode? parseMode = ParseMode.MarkdownV2) => 
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: text,
                    parseMode: parseMode,
                    replyToMessageId: replyTo,
                    cancellationToken: cancellationToken);

        public static int GetRandomNumber(int from, int to) => new Random().Next(from, to);

    }

    public class PingService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stop)
        {
            while(!stop.IsCancellationRequested)
            {
                var periodMs = 1000 * 60 * 10; // ping each 10 minutes
                await Task.Delay(periodMs, stop);
                using var client = new HttpClient();
                try {
                    var content = await client.GetStringAsync("https://testwebapp182.azurewebsites.net/");
                    Console.WriteLine(content);
                } catch (HttpRequestException hre) {
                    Console.WriteLine(hre);
                }
            }
        }
    }

    public class AirAlertService : BackgroundService
    {
        private static bool KyivAlert = false;
        protected override async Task ExecuteAsync(CancellationToken stop)
        {
            while(!stop.IsCancellationRequested)
            {
                await Task.Delay(5000, stop);
                using var client = new HttpClient();

                var alert = false;
                try {
                    var content = await client.GetStringAsync("https://t.me/s/air_alert_ua");
                    using (StringReader reader = new StringReader(content))
                    {
                        string line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null)
                            {
                                if (line.Contains("Повітряна тривога в м. Київ")) {
                                    alert = true;
                                }
                                if (line.Contains("Відбій тривоги в м. Київ")) {
                                    alert = false;
                                }
                            }
                        } while (line != null);
                    }
                    if (KyivAlert != alert) {
                        var stickerSender = new StickerChecker();
                        using var cts = new CancellationTokenSource();
                        var botClient = new TelegramBotClient("5494507018:AAH_cO5fyahYu6w_AseDyZHvT7vZkmGMFvo");
                        await botClient.SendStickerAsync(
                            chatId:  -1001344803304,
                            sticker: stickerSender.GetStickerLink(alert ? "тривога" : "відбій"),
                            cancellationToken: cts.Token);
                        KyivAlert = alert;
                    }
                } catch (HttpRequestException hre) {
                    Console.WriteLine(hre);
                }
            }
        }
    }
}
