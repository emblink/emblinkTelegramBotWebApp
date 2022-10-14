using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Microsoft.Extensions.Configuration;
using BarracudaTestBot;
using BarracudaTestBot.Checkers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<PingService>();
builder.Services.AddHostedService<AirAlertService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Console.OutputEncoding = System.Text.Encoding.UTF8;

var dateOfStart = DateTime.UtcNow;

// var builder = new ConfigurationBuilder()
//             .AddJsonFile($"appsettings.json", false, true)
//             .AddJsonFile($"appsettings.Local.json", true, true)
//             .AddEnvironmentVariables();
// var configuration = builder.Build();
// var token = configuration.GetValue<string>("TelegramToken");

var botClient = new TelegramBotClient("5494507018:AAH_cO5fyahYu6w_AseDyZHvT7vZkmGMFvo");

using var cts = new CancellationTokenSource();


List<long> MutedInChats = new List<long>();

var Commands = new List<BotCommand>();
var command = new BotCommand();
command.Command = "off"; command.Description = "вимкнути бота в чаті";
Commands.Add(command);
command = new BotCommand();
command.Command = "on"; command.Description = "ввімкнути бота в чаті";
Commands.Add(command);

await botClient.SetMyCommandsAsync(Commands, cancellationToken: cts.Token);
// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

// Console.WriteLine($"Start listening for @{me.Username}");
// Console.ReadLine();
app.Run();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message?.Date < dateOfStart) return;

    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    // Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    var commandsList = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    var checkedWord = new WordChecker().GetAnswerByCommand(messageText);

    if (!string.IsNullOrEmpty(checkedWord))
    {
        await botClient.SendText(message.ReplyToMessage?.MessageId, chatId, checkedWord, cancellationToken);
        return;
    }

    if (commandsList[0] != "бот") return;

    if (commandsList[1] == "тривога" || commandsList[1] == "відбій") {
        return;
    }
    var stickerSender = new StickerChecker();
    if (commandsList.Length > 1 && stickerSender.IsStickerCommand(commandsList[1]))
    {
        await botClient.SendStickerAsync(
            chatId: chatId,
            sticker: stickerSender.GetStickerLink(commandsList[1]),
            cancellationToken: cancellationToken);
        return;
    }

    var correctMessageText = messageText.Remove(0, 3).TrimStart();

    var answer = message.ReplyToMessage?.MessageId != null ? $"у відповідь:" : "";

    var text = $"@{message.From.Username} {answer} {correctMessageText}";

    await botClient.DeleteMessageAsync(chatId, message.MessageId);
    await botClient.SendText(message.ReplyToMessage?.MessageId, chatId, text, cancellationToken, null);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    // Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
