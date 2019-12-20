using MihaZupan;
using System;
using System.Collections.Generic;
using DeadBot.Models;
using Telegram.Bot;
using DeadBot.Notifications;

namespace DeadBot.ManagersAndFactory
{
    class Factory
    {
        static Factory instance;
        public static Factory Instance => instance ?? (instance = new Factory());
        private Factory() { }

        readonly HttpToSocks5Proxy proxy = new HttpToSocks5Proxy("46.101.123.77", 7890, "proxyuser", "2SASZwRfJbd24udA");
        static ITelegramBotClient client;
        public ITelegramBotClient GetClient => client ?? 
                (client = new TelegramBotClient("892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q", proxy)
                { Timeout = TimeSpan.FromSeconds(5) });

        static Dictionary<long, DeadLine> UsersAndUnfinishedDeadlines; 
        static List<long> WhoWantedToDelete;
        public Dictionary<long, DeadLine> GetUsersAndUnfinishedDeadlines => UsersAndUnfinishedDeadlines ?? 
                                            (UsersAndUnfinishedDeadlines = new Dictionary<long, DeadLine>());
        public List<long> GetWhoWantedToDelete => WhoWantedToDelete ?? (WhoWantedToDelete = new List<long>());

        Notificater notificater;
        public Notificater GetNotificater => notificater ?? (notificater = new Notificater());

    }
}
