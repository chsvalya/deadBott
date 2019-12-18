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
        static Dictionary<long, DeadLine> UsersAndUnfinishedDeadlines = new Dictionary<long, DeadLine>();
        static List<long> WhoWantedToDelete = new List<long>();
        static ApplicationContext context = new ApplicationContext();
        static void Main()
        {
            var proxy = new HttpToSocks5Proxy(
                "46.101.123.77", 7890, "proxyuser", "2SASZwRfJbd24udA");

            client = new TelegramBotClient("892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q", proxy) { Timeout = TimeSpan.FromSeconds(5) };

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"id: {me.Id}, name: {me.FirstName}");

            //еще не работает из-за startdate
            //var notificater = new Notificater(client);

            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadKey();
            client.StopReceiving();
            /* я же правильно сделала?*/
            //Task.Run(() => {
            //    while (true)
            //    {
            //        notificater.Sending();
            //    }
            //        });
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
            List<DeadLine> userDeadlines;
            using (context = new ApplicationContext())
            {
                userDeadlines = context.DeadLines.Where(d => d.ChatId == id).ToList();
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

            if (UsersAndUnfinishedDeadlines.Keys.Count(x => x == id) == 0 && !WhoWantedToDelete.Contains(id))
            {
                if (text == "Add")
                {
                    await client.SendTextMessageAsync(id, "Enter name of deadline").ConfigureAwait(false);
                    UsersAndUnfinishedDeadlines.Add(id, new DeadLine() {ChatId=id });
                }
                else if (text == "Show all")
                {
                    var answer = "";
                    foreach (var deadline in userDeadlines)
                    {
                        answer += $"{deadline.Name} at {deadline.DateTime} \n";
                    }
                    await client.SendTextMessageAsync(id, $"These are all your deadlines {answer}.").ConfigureAwait(false);
                }

                else if(text == "Delete")
                {
                    var answer = "";
                    var counter = 1;

                    foreach (var deadline in userDeadlines)
                    {
                        answer += $"{counter}: {deadline.Name} at {deadline.DateTime} \n";
                        counter++;
                    }
                    await client.SendTextMessageAsync(id, $"These are all your deadlines {answer}. " +
                                                        $"Choose what to delete").ConfigureAwait(false);
                    WhoWantedToDelete.Add(id);

                    
                }
                else
                    await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "What are we doing with deadlines?",
                                      ParseMode.Default, false, false, 0, mainKeyboard);
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
                            await client.SendTextMessageAsync(id, "This is not a number!");
                            return;
                        }
                    }
                    try {
                        DeadLine deleted = userDeadlines[userResponce - 1];
                        await client.SendTextMessageAsync(id, $"Deadline \'{deleted.Name}\' deleted")
                                                                .ConfigureAwait(false);
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
                        await client.SendTextMessageAsync(id, "You picked something wrong");
                        return;
                    }
                    
                }
                DeadLine unfinished = new DeadLine() { ChatId = id};
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
                        await client.SendTextMessageAsync(id, "Enter deadline itself in the following format" +
                                                                          " YYYY-MM-DD HH:MM:SS").ConfigureAwait(false);
                    }
                    else
                    { await client.SendTextMessageAsync(id, "Please choose notification frequency from keyboard!");
                    }
                }
                else if (unfinished.DateTime is null)
                {
                    DateTime Date;
                    if (DateTime.TryParse(text, out Date) && Date > DateTime.Now)
                    {
                        unfinished.DateTime = Date;
                        Console.WriteLine($"added dt: {text}");
                        await client.SendTextMessageAsync(id, "Enter the deadline itself in the following format" +
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
                    //unfinished.StartDate = DateTime.Parse(text);
                    // if start date < datetime
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
