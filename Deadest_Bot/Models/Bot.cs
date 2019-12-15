using DeadBot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace DeadBot.Models
{
    /*инициализация бота и его команд */
    static class Bot
    {
        private static TelegramBotClient client;
        private static List<Command> commandList;

        public static IReadOnlyList<Command>  Commands { get => commandList.AsReadOnly(); }

        public static async Task<TelegramBotClient> Get() 
        {
            if (client != null)
            {
                return client;
            }
            /*здесь команды, которые наследуются от Command */
            commandList = new List<Command>();
            client = new TelegramBotClient(BotSettings.Key);
            await client.SetWebhookAsync("");

            return client;
        }
    }
}
