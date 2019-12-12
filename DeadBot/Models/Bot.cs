using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace DeadBot.Models
{
    /*инициализация бота и его команд */
    static class Bot
    {
        private static TelegramBotClient client;

        public static async Task<TelegramBotClient> Get() 
        {
            if (client != null)
            {
                return client;
            }
            client = new TelegramBotClient(BotSettings.Key);
            await client.SetWebhookAsync("");

            return client;
        }
    }
}
