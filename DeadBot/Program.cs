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
using System.Data.Entity;
using DeadBot.Commands;
using System.IO;

namespace DeadBot
{

    class Program
    {
        static ITelegramBotClient client;
        static readonly Dictionary<long, DeadLine> UsersAndUnfinishedDeadlines = new Dictionary<long, DeadLine>();
        static readonly List<long> WhoWantedToDelete = new List<long>();
        static ApplicationContext context = new ApplicationContext();
        static void Main()
        {
            var proxy = new HttpToSocks5Proxy(
                "46.101.123.77", 7890, "proxyuser", "2SASZwRfJbd24udA");

            client = new TelegramBotClient("892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q", proxy) { Timeout = TimeSpan.FromSeconds(5) };

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"id: {me.Id}, name: {me.FirstName}");

            
            var notificater = new Notificater(client);

            Task.Run(() =>
            {
                while (true)
                {
                    notificater.Sending();
                }
            });

            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadKey();
            client.StopReceiving();
        }

        static string WriteDeadlines(IEnumerable<DeadLine> deadlines)
        {
            var answer = "";
            var counter = 1;
            foreach (var deadline in deadlines)
            {
                answer += $"{counter}: {deadline.Name} on {deadline.DateTime}\n";
            }
            return answer;
        }


        static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs?.Message;
            var text = message?.Text;

            if (text == null)
                return;

            int userId = default;
            var id = message.Chat.Id;
            var tgUser = message.From;

            List<DeadLine> userDeadlines;
            using (context = new ApplicationContext())
            {
                userDeadlines = context.DeadLines.Where(d => d.ChatId == id).ToList();
                var dbuser = context.Users
                    .FirstOrDefault(value => value.TelegramId == tgUser.Id);

                if (dbuser == default)
                {
                    dbuser = new User
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

            if (UsersAndUnfinishedDeadlines.Keys.Count(x => x == id) == 0 && !WhoWantedToDelete.Contains(id))
            {
                if (text == "Add")
                {
                    await client.SendTextMessageAsync(id, "Enter name of deadline.").ConfigureAwait(false);
                    UsersAndUnfinishedDeadlines.Add(id, new DeadLine {ChatId=id });
                }

                else if (text == "Show all")
                {
                    await client.SendTextMessageAsync(id, $"These are all your deadlines:\n{WriteDeadlines(userDeadlines)}" +
                        $"What's next?", ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);
                }

                else if(text == "Delete" && userDeadlines.Count() != 0)
                {
                    await client.SendTextMessageAsync(id, $"These are all your deadlines:\n" +
                        $"{WriteDeadlines(userDeadlines)} Choose deadline number to delete").ConfigureAwait(false);
                    WhoWantedToDelete.Add(id);
                }

                else
                    await client.SendTextMessageAsync(id, "What are we doing with deadlines?",
                                      ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);
            }

            else
            {
                if (WhoWantedToDelete.Contains(id))
                {
                    var userResponce = 0;

                    if (userResponce == 0)
                    {
                        if(!int.TryParse(text,out userResponce))
                        {
                            await client.SendTextMessageAsync(id, "This is not a number!").ConfigureAwait(false);
                            return;
                        }
                    }
                    try {
                        DeadLine deleted = userDeadlines[userResponce - 1];
                        await client.SendTextMessageAsync(id, $"Deadline \'{deleted.Name}\' deleted!\nWhat's next?",
                                ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);
                        using (var context = new ApplicationContext())
                        {
                            context.DeadLines.Attach(deleted);
                            context.DeadLines.Remove(deleted);
                            await context.SaveChangesAsync();
                        }

                        WhoWantedToDelete.Remove(id);
                        return;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        await client.SendTextMessageAsync(id, "You picked something wrong!").ConfigureAwait(false);
                        return;
                    }
                }

                DeadLine unfinished = new DeadLine { ChatId = id};
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
                    if (!string.IsNullOrEmpty(text) && (text == "Twice a day" || text == "Once a day" || text == "Every 5 hours"))
                    {
                        unfinished.NotificationFrequency = text;
                        Console.WriteLine($"added freq: {text}");
                        await client.SendTextMessageAsync(id, "When is the deadline? In the following format" +
                                                                          " YYYY-MM-DD HH:MM:SS").ConfigureAwait(false);
                    }
                    else
                        await client.SendTextMessageAsync(id, "Please choose notification frequency from keyboard!");
                }

                else if (unfinished.DateTime is null)
                {
                    if (DateTime.TryParse(text, out DateTime Date) && Date > DateTime.Now)
                    {
                        unfinished.DateTime = Date;
                        Console.WriteLine($"added dt: {text}");
                        await client.SendTextMessageAsync(id, "Wnen should I start to remind? In the following format" +
                                                                          " YYYY-MM-DD HH:MM:SS").ConfigureAwait(false);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(id, "Please enter the date in the following format" +
                                                                          " YYYY-MM-DD HH:MM:SS").ConfigureAwait(false);
                    }

                }
                else if (unfinished.StartDate is null)
                {
                    DateTime date;
                    if (DateTime.TryParse(text, out date))
                    {
                        unfinished.StartDate = date;
                        Console.WriteLine($"added start: {text}");
                    }
                    else
                        await client.SendTextMessageAsync(id, "Please enter the date in the following format" +
                                                                          " YYYY-MM-DD HH:MM:SS").ConfigureAwait(false);
                }
                if (unfinished.Name != null && unfinished.NotificationFrequency != null &&
                        unfinished.StartDate != null && unfinished.DateTime != null)
                {
                    await client.SendTextMessageAsync(id, "Deadline is added, thanks and good luck!\nWhat's next?", 
                        ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);

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
