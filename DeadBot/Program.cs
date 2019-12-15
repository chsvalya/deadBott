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
        static Dictionary<long, List<DeadLine>> UsersAndDeadlines = new Dictionary<long, List<DeadLine>>();
        static void Main()
        {
            var proxy = new HttpToSocks5Proxy(
                "46.101.123.77", 7890, "proxyuser", "2SASZwRfJbd24udA");

            client = new TelegramBotClient("892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q") { Timeout = TimeSpan.FromSeconds(10) };

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
            var id = messageEventArgs.Message.Chat.Id;
            List<DeadLine> deadLines;
            try
            {
                deadLines = UsersAndDeadlines.First(u => u.Key == id).Value;
            }
            catch
            {
                UsersAndDeadlines.Add(id, new List<DeadLine>());
                deadLines = UsersAndDeadlines.First(u => u.Key == id).Value;
            }

            var unfinishedDeadlines = deadLines.Where(d => d.Name is null || d.NotificationFrequency is null
                                                        || d.StartDate is null || d.DateTime is null).ToList();
            if (unfinishedDeadlines.Count() == 0)
            {
                unfinishedDeadlines.Add(new DeadLine());
            }
            foreach (var deadline in unfinishedDeadlines)
            {
                if (deadline.Name is null)
                {
                    await client.SendTextMessageAsync(id, "Enter name of deadline").ConfigureAwait(false);
                    if (!String.IsNullOrEmpty(text))
                        deadline.Name = text;
                    Console.WriteLine($"added name: {text}");
                }
                if (deadline.NotificationFrequency is null)
                {
                    await client.SendTextMessageAsync(id, "Choose frequency of reminding",
                                                     ParseMode.Default, false, false, 0, frequencyKeyboard).ConfigureAwait(false);
                    deadline.NotificationFrequency = text;
                    Console.WriteLine($"added freq: {text}");
                }
                if (deadline.DateTime is null)
                {
                    await client.SendTextMessageAsync(id, "Enter deadline itself in the following format" +
                                                                  "YYYY-MM-DD HH-MM-SS").ConfigureAwait(false);
                    deadline.DateTime = DateTime.Parse(text);
                    Console.WriteLine($"added dt: {text}");
                }
                if (deadline.StartDate is null)
                {
                    await client.SendTextMessageAsync(id, "Choose when start to remind in the following format",
                                                    ParseMode.Default, false, false, 0, startDateKeyboard).ConfigureAwait(false);
                    deadline.StartDate = text;
                    Console.WriteLine($"added start: {text}");
                }

            }
            
            Console.WriteLine($"received: {text}");
            if (text == "/start")
                await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "What are we doing with deadlines?",
                                                ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);
            
            
            ////if (!users.ContainsKey(message.Chat.Id))
            ////    users.Add(message.Chat.Id, new List<DeadLine>()); ;
            ////var user = users.First(u => u.Key == message.Chat.Id);

                //int userId = default;
                //if (messageEventArgs.Message?.Type == MessageType.Text)
                //{
                //    if (messageEventArgs.Message.Text == "/start")
                //    {
                //        var tgUser = messageEventArgs.Message.From;
                //        using (var context = new ApplicationContext())
                //        {
                //            var dbuser = context.Users
                //                .Where(value => value.TelegramId == tgUser.Id)
                //                .SingleOrDefault();

                //            if (dbuser == default)
                //            {
                //                dbuser = new User()
                //                {
                //                    FirstName = tgUser.FirstName,
                //                    IsBot = tgUser.IsBot,
                //                    LastName = tgUser.LastName,
                //                    Username = tgUser.Username,
                //                    TelegramId = tgUser.Id
                //                };
                //                context.Users.Add(dbuser);
                //                await context.SaveChangesAsync();
                //            }

                //            userId = dbuser.Id;
                //        }
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "What are we doing with deadlines?",
                //                                    ParseMode.Default, false, false, 0, mainKeyboard);
                //    }

                //    if (messageEventArgs.Message.Text == "Add")
                //    {

                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Enter name of deadline");
                //        /* надо разобраться, как заставить его ждать сообщений, чтобы который ниже работал корректно*/

                //        string name = messageEventArgs.ToString();
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Enter deadline itself in the following format" +
                //                                                   "YYYY-MM-DD HH-MM-SS");
                //        //DateTime deadline = DateTime.Parse(messageEventArgs.ToString()); // parsing error
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Enter priprity from 1 to 3");
                //        //int priority = int.Parse(messageEventArgs.ToString()); // parsing error
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Choose frequency of reminding",
                //                                      ParseMode.Default, false, false, 0, frequencyKeyboard);
                //        string frequency = messageEventArgs.ToString();
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Choose when start to remind",
                //                                      ParseMode.Default, false, false, 0, startDateKeyboard);
                //        DateTime startDate = DateTime.Now;
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Deadline added successfully!");

                //        using (var context = new ApplicationContext())
                //        {
                //            var dbUser = context.Users
                //                .Where(value => value.Id == userId)
                //                .Single();

                //            dbUser.DeadLines.Add(new DeadLine
                //            {
                //                ChatId = messageEventArgs.Message.Chat.Id,
                //                Name = name,
                //                DateTime = new DateTime(),
                //                Priority = 0,
                //                NotificationFrequency = frequency,
                //                StartDate = startDate
                //            });
                //            await context.SaveChangesAsync();
                //        }
                //    }

                //    if (messageEventArgs.ToString() == "Edit")
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "What to change?",
                //                                      ParseMode.Default, false, false, 0, null);
                //    if (messageEventArgs.ToString() == "Delete")
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "Are you sure?",
                //                                      ParseMode.Default, false, false, 0, null);
                //    if (messageEventArgs.ToString() == "Show all")
                //    {
                //        await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "These are your deadlines");

                //    }
                //}
        }
    }
}
