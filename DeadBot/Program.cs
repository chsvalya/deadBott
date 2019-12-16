using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using static DeadBot.Keyboards.Keyboards;
using DeadBot.Enums;
using DeadBot.ManageMMSQL;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using MihaZupan;

namespace DeadBot
{

    class Program
    {
        static ITelegramBotClient client;
        static Dictionary<long, DeadLine> UsersAndUnfinishedDeadlines = new Dictionary<long, DeadLine>();
        static ApplicationContext context = new ApplicationContext();
        static void Main()
        {
            var proxy = new HttpToSocks5Proxy(
                "46.101.123.77", 7890, "proxyuser", "2SASZwRfJbd24udA");

            client = new TelegramBotClient("892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q") { Timeout = TimeSpan.FromSeconds(5) };

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"id: {me.Id}, name: {me.FirstName}");

            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadKey();
            client.StopReceiving();
        }


        /* вообще, все что не в main надо перенести в другой класс */
        static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var text = messageEventArgs?.Message?.Text;
            if (text == null)
                return;
            int userId = default;
            var id = messageEventArgs.Message.Chat.Id;
            var tgUser = messageEventArgs.Message.From;
            var userDeadlines = context.DeadLines.Where(d => d.ChatId == id).ToList();
            using (context = new ApplicationContext())
            {
                var dbuser = context.Users
                    .First(value => value.TelegramId == tgUser.Id);

                if (dbuser == default)
                {
                    dbuser = new User()
                    {
                        FirstName = tgUser.FirstName,
                        IsBot = tgUser.IsBot,
                        LastName = tgUser.LastName,
                        Username = tgUser.Username,
                        TelegramId = tgUser.Id
                    };
                    context.Users.Add(dbuser);
                    await context.SaveChangesAsync();
                }
                userId = dbuser.Id;
            }

            if (UsersAndUnfinishedDeadlines.Keys.Count(x => x == id) == 0)
            {
                if (text == "Add")
                {
                    await client.SendTextMessageAsync(id, "Enter name of deadline").ConfigureAwait(false);
                    UsersAndUnfinishedDeadlines.Add(id, new DeadLine(messageEventArgs.Message.Chat.Id));
                }
                else if (text == "Show all")
                {
                    var answer = "";
                    foreach (var deadline in userDeadlines)
                    {
                        answer += $"{deadline.Name} at {deadline.DateTime} /n";
                    }
                    await client.SendTextMessageAsync(id, $"These are all your deadlines {answer}.").ConfigureAwait(false);
                }

                else if(text == "Delete")
                {
                    var answer = "";
                    var counter = 1;
                    var userResponce = 0;
                    foreach (var deadline in userDeadlines)
                    {
                        answer += $"{counter}: {deadline.Name} at {deadline.DateTime} /n";
                        counter++;
                    }
                    await client.SendTextMessageAsync(id, $"These are all your deadlines {answer}. " +
                                                        $"Choose what to delete").ConfigureAwait(false);
                    if (userResponce == 0)
                    {
                        userResponce = int.Parse(text);
                    }
                    await client.SendTextMessageAsync(id, $"Deadline {userDeadlines[userResponce - 1].Name} deleted")
                                                            .ConfigureAwait(false);
                    userDeadlines.RemoveAt(userResponce - 1);
                }
                else
                    await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "What are we doing with deadlines?",
                                      ParseMode.Default, false, false, 0, mainKeyboard);
            }
            else
            {
                DeadLine unfinished = new DeadLine(0);
                UsersAndUnfinishedDeadlines.TryGetValue(id, out unfinished);
                if (unfinished.Name == null) 
                {
                    if (!string.IsNullOrEmpty(text))
                        unfinished.Name = text;
                    Console.WriteLine($"added name: {text}");
                    await client.SendTextMessageAsync(id, "Choose frequency of reminding",
                                                         ParseMode.Default, false, false, 0, frequencyKeyboard).ConfigureAwait(false);
                }
                else if (unfinished.NotificationFrequency is null)
                {
                    unfinished.NotificationFrequency = text;
                    Console.WriteLine($"added freq: {text}");
                    await client.SendTextMessageAsync(id, "Enter deadline itself in the following format" +
                                                                      " YYYY-MM-DD HH:MM:SS").ConfigureAwait(false);
                }
                else if (unfinished.DateTime is null)
                {
                    unfinished.DateTime = DateTime.Parse(text);
                    Console.WriteLine($"added dt: {text}");
                    await client.SendTextMessageAsync(id, "Choose when start to remind in the following format",
                                                        ParseMode.Default, false, false, 0, startDateKeyboard).ConfigureAwait(false);
                }
                else if (unfinished.StartDate is null)
                {
                    unfinished.StartDate = text;
                    Console.WriteLine($"added start: {text}");
                }
                if (unfinished.Name != null && unfinished.NotificationFrequency != null &&
                        unfinished.StartDate != null && unfinished.DateTime != null)
                {
                    await client.SendTextMessageAsync(id, "Deadline is added");
                    using (context = new ApplicationContext())
                    {
                        context.DeadLines.Add(UsersAndUnfinishedDeadlines[id]);
                        context.SaveChanges();
                    }
                    UsersAndUnfinishedDeadlines.Remove(id);
                }
            }
        }
    }
}
