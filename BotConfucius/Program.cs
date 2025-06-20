using Telegram.Bot;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DatabaseLibrary.Models;


IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();


// телеграм бот який відповідає на будь яке питання цитатами Конфуція

var token = configuration["TelegramBotToken"];
var bot = new TelegramBotClient(token);

//bot.OnMessage += Bot_OnMessage;
var cts = new CancellationTokenSource();
var cancelationToken = cts.Token;
var receiverOptions = new Telegram.Bot.Polling.ReceiverOptions
{
    AllowedUpdates = Array.Empty<Telegram.Bot.Types.Enums.UpdateType>(), // Отримувати всі типи оновлень
    DropPendingUpdates = true, // Скинути всі очікувані оновлення
    Limit = 100 // Максимальна кількість оновлень за один запит
};

var me = await bot.GetMe(cancelationToken);
Console.WriteLine($"Bot {me.FirstName} started");

await bot.ReceiveAsync(
    updateHandler: Bot_OnMessage, // Обробник повідомлень
    errorHandler: Bot_OnError, // Обробник помилок
    receiverOptions: receiverOptions,
    cancellationToken: cancelationToken
);



Console.ReadLine(); // Залишаємо програму запущеною, поки не буде натиснуто Enter

cts.Cancel(); // Скасовуємо отримання оновлень при завершенні програми

string GetRandomQuote()
{
    var contextOptions = new DbContextOptionsBuilder<BotContext>()
        .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        .Options;

    using (var db = new DatabaseLibrary.Models.BotContext(contextOptions))
    {
        var quotes = db.Quotes.ToList();
        if (quotes.Count == 0)
        {
            return "No quotes available.";
        }

        var random = new Random();
        int index = random.Next(quotes.Count);
        return quotes[index].Text;
    }
}

async Task Bot_OnError(ITelegramBotClient bot, Exception exception, CancellationToken ct)
{
    Console.WriteLine($"Error: {exception.Message}");
    await Task.CompletedTask; // Возвращаем завершённую задачу, чтобы не блокировать поток
}

async Task Bot_OnMessage(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken ct)
{
    if (update.Message == null)
    {
        return;
    }
    await bot.SendMessage(update.Message.Chat.Id, GetRandomQuote());    
}