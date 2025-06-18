using Telegram.Bot;

// телеграм бот який відповідає на будь яке питання цитатами Конфуція

var token = "7619230076:AAF0j_4Lbbwp8lKzgb8UZ1tJYx-A9f4Nxzg";
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
    errorHandler: null, // Тут можна вказати обробник помилок, якщо потрібно
    receiverOptions: receiverOptions,
    cancellationToken: cancelationToken
);



Console.ReadLine(); // Залишаємо програму запущеною, поки не буде натиснуто Enter

cts.Cancel(); // Скасовуємо отримання оновлень при завершенні програми

string GetRandomQuote()
{
    //return "Це тестова цитата Конфуція."; // Тут має бути логіка для отримання випадкової цитати з бази даних

    using (var db = new DatabaseLibrary.Models.BotContext())
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

async Task Bot_OnMessage(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken ct)
{
    await bot.SendMessage(update.Message.Chat.Id, GetRandomQuote());    
}



/*
  
Data Source=SILVERSTONE\SQLEXPRESS;Initial Catalog=MyAcademy;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;



  
 */