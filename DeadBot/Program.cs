using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace DeadBot
{
    class Program
    {
        private static TelegramBotClient client;
        private const string token = "892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q";

        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);
            client.OnMessage += BotOnMessageReceived;
            client.OnMessageEdited += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }
        private async static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message?.Type == MessageType.Text)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "You're beatiful !");
            }
        }
    }
}
