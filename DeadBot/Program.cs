using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using static DeadBot.Keyboards.Keyboards;
using DeadBot.Enums;

namespace DeadBot
{
    class Program
    {
        private static TelegramBotClient client;
        private const string token = "892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q";
        private static List<User> users = new List<User>();
            
        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);
            
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadKey();
            client.StopReceiving();
        }

        /* вообще, все что не в main надо перенести в другой класс */
        private async static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (users.FirstOrDefault(u => u.ChatId == message.Chat.Id) is null)
                users.Add(new User(message.Chat.Id));
            var user = users.First(u => u.ChatId == message.Chat.Id);
            if (message?.Type == MessageType.Text)
                if (message.Text == "/start")
                    await client.SendTextMessageAsync(message.Chat.Id, "What are we doing with deadlines?",
                                                      ParseMode.Default, false, false, 0, mainKeyboard);
                if (message.Text == "Add")
                {
                await client.SendTextMessageAsync(message.Chat.Id, "Enter name of deadline");
                /* надо разобраться, как заставить его ждать сообщений, чтобы который ниже работал корректно */

                //string name = message.Text;
                //await client.SendTextMessageAsync(message.Chat.Id, "Enter deadline itself in the following format" +
                //                                                   "YYYY-MM-DD HH-MM-SS");
                //DateTime deadline = DateTime.Parse(message.Text);
                //await client.SendTextMessageAsync(message.Chat.Id, "Enter priprity from 1 to 3");
                //int priority = int.Parse(message.Text);
                //await client.SendTextMessageAsync(message.Chat.Id, "Choose frequency of reminding",
                //                                      ParseMode.Default, false, false, 0, frequencyKeyboard);
                //string frequency = message.Text;
                //await client.SendTextMessageAsync(message.Chat.Id, "Choose when start to remind",
                //                                      ParseMode.Default, false, false, 0, startDateKeyboard);
                //string startDate = message.Text;
                //await client.SendTextMessageAsync(message.Chat.Id, "Deadline added successfully!");
                //user.DeadLines.Add(new DeadLine
                //{
                //    Name = name,
                //    DateTime = deadline,
                //    Priority = priority,
                //    NotificationFrequency = frequency,
                //    StartDate = startDate
                //});
                } 
                    
                if (message.Text == "Edit")
                    await client.SendTextMessageAsync(message.Chat.Id, "What to change?",
                                                      ParseMode.Default, false, false, 0, null);
                if (message.Text == "Delete")
                    await client.SendTextMessageAsync(message.Chat.Id, "Are you sure?",
                                                      ParseMode.Default, false, false, 0, null);
                if (message.Text == "Show all")
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "These are your deadlines");
                    foreach(var deadline in user.DeadLines)
                    {
                    await client.SendTextMessageAsync(message.Chat.Id, $"{deadline.Name} on {deadline.DateTime}");
                    }
                }     
        }
    }
}
