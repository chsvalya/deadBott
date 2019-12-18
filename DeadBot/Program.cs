using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using static DeadBot.UsefulMethods.AnswerManager;
using static DeadBot.UsefulMethods.ContextManager;
using System.Threading.Tasks;
using MihaZupan;
using DeadBot.Commands;

namespace DeadBot
{
    class Program
    {
        static ITelegramBotClient client;
        static readonly Dictionary<long, DeadLine> UsersAndUnfinishedDeadlines = new Dictionary<long, DeadLine>();
        static readonly List<long> WhoWantedToDelete = new List<long>();

        static void Main()
        {
            var proxy = new HttpToSocks5Proxy("46.101.123.77", 7890, "proxyuser", "2SASZwRfJbd24udA");

            client = new TelegramBotClient("892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q", proxy)
                        { Timeout = TimeSpan.FromSeconds(5) };

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"id: {me.Id}, name: {me.FirstName}");
            
            var notificater = new Notificater(client);

            Task.Run(() =>
            {
                while (true)
                    notificater.Sending();
            });

            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();

            Console.ReadKey();
            client.StopReceiving();
        }

        static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs?.Message;
            var text = message?.Text;

            if (text == null)
                return;

            int userId = default;
            var chatId = message.Chat.Id;
            var tgUser = message.From;

            List<DeadLine> userDeadlines = new List<DeadLine>();
            GetUser(userDeadlines, chatId, tgUser, userId);

            if (IsOldUserWithAllDeadlinesFinished(UsersAndUnfinishedDeadlines, WhoWantedToDelete, chatId))
            {
                if (text == "Add")
                    AddReact(client, UsersAndUnfinishedDeadlines, chatId);

                else if (text == "Show all")
                    ShowAllReact(client, userDeadlines, chatId);

                else if (text == "Delete" && userDeadlines.Count() != 0)
                    DeleteReact(client, WhoWantedToDelete, userDeadlines, chatId);

                else
                    ClassicGreetings(client, chatId);
            }

            else
            {
                if (WhoWantedToDelete.Contains(chatId))
                {
                    int userResponce = SelectedDeadline(client, chatId, text);

                    try
                    {
                        DeadLine deleted = userDeadlines[userResponce - 1];
                        DeleteDeadline(client, chatId, deleted, WhoWantedToDelete);
                        DeleteFromBd(deleted);
                        return;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        WrongInputReact(client, chatId);
                        return;
                    }
                }

                UsersAndUnfinishedDeadlines.TryGetValue(chatId, out DeadLine unfinished);

                if (unfinished.Name == null)
                    ConfigureName(client, unfinished, chatId, text);

                else if (unfinished.NotificationFrequency is null)
                    ConfigureFrequency(client, unfinished, chatId, text);

                else if (unfinished.DateTime is null)
                    ConfigureDateTime(client, unfinished, chatId, text);

                else if (unfinished.StartDate is null)
                    ConfigureStartDate(client, unfinished, chatId, text);

                if (IsFullDeadline(unfinished))
                {
                    AddDeadline(client, UsersAndUnfinishedDeadlines, chatId);
                    AddToBd(UsersAndUnfinishedDeadlines, chatId);
                }
            }
        }
    }
}
