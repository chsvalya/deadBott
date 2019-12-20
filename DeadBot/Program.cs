using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using static DeadBot.ManagersAndFactory.AnswerManager;
using static DeadBot.ManagersAndFactory.ContextManager;
using System.Threading.Tasks;
using DeadBot.Notifications;
using DeadBot.ManageMMSQL;
using DeadBot.ManagersAndFactory;

namespace DeadBot
{
    class Program
    {
        static readonly ITelegramBotClient client = Factory.Instance.GetClient;
        static readonly Dictionary<long, DeadLine> UsersAndUnfinishedDeadlines = Factory.Instance.GetUsersAndUnfinishedDeadlines;
        static readonly List<long> WhoWantedToDelete = Factory.Instance.GetWhoWantedToDelete;
        static readonly Notificater notificater = Factory.Instance.GetNotificater;

        static void Main()
        {
            var me = client.GetMeAsync().Result;
            Console.WriteLine($"id: {me.Id}, name: {me.FirstName}");

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
            using (var context = new ApplicationContext())
            {
                userDeadlines = context.DeadLines.Where(x => x.ChatId == chatId).ToList();
            }
            GetUser(tgUser, userId);

            if (IsOldUserWithAllDeadlinesFinished(chatId))
            {
                if (text == "Add")
                    AddReact(chatId);

                else if (text == "Show all")
                    ShowAllReact(userDeadlines, chatId);

                else if (text == "Delete" && userDeadlines.Count() != 0)
                    DeleteReact(userDeadlines, chatId);

                else
                    ClassicGreetings(chatId);
            }

            else
            {
                if (WhoWantedToDelete.Contains(chatId))
                {
                    int userResponce = SelectedDeadline(chatId, text);

                    try
                    {
                        DeadLine deleted = userDeadlines[userResponce - 1];
                        DeleteDeadline(chatId, deleted);
                        DeleteFromBd(deleted);
                        return;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        WrongInputReact(chatId);
                        return;
                    }
                }

                UsersAndUnfinishedDeadlines.TryGetValue(chatId, out DeadLine unfinished);

                if (unfinished.Name == null)
                    ConfigureName(unfinished, chatId, text);

                else if (unfinished.NotificationFrequency is null)
                    ConfigureFrequency(unfinished, chatId, text);

                else if (unfinished.DateTime is null)
                    ConfigureDateTime(unfinished, chatId, text);

                else if (unfinished.StartDate is null)
                    ConfigureStartDate(unfinished, chatId, text);

                if (IsFullDeadline(unfinished))
                {
                    AddDeadline(chatId);
                    AddToBd(chatId);
                }
            }
        }
    }
}
