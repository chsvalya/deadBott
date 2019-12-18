using DeadBot.ManageMMSQL;
using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace DeadBot.Commands
{
    class Notificater
    {
        static ITelegramBotClient client;
        static List<Action> senders = new List<Action>();
        static List<DeadLine> once;
        static List<DeadLine> twice;
        static List<DeadLine> hours_5;

        public Notificater(ITelegramBotClient c)
        {
            client = c;
            FillIn();
        }
        public async void FillIn()
        {
            using (var contxt = new ApplicationContext())
            {
                once = await contxt.DeadLines.Where(x => x.NotificationFrequency == "Once a day" && DateTime.Parse(x.StartDate) >= DateTime.Now).ToListAsync();
                twice = await contxt.DeadLines.Where(x => x.NotificationFrequency == "Twice a day" && DateTime.Parse(x.StartDate) >= DateTime.Now).ToListAsync();
                hours_5 = await contxt.DeadLines.Where(x => x.NotificationFrequency == "Every 5 hours" && DateTime.Parse(x.StartDate) >= DateTime.Now).ToListAsync();
            }
            senders = new List<Action>() { OnceSender, TwiceSender, Hours };
        }
        private async void OnceSender()
        {
            foreach (var u in once)
            {
                await client.SendTextMessageAsync(u.User.TelegramId, $"Hey, you will be DEAD in a {(u.DateTime - DateTime.Now).ToString()}, so do this task dear");
            }
        }
        private async void TwiceSender()
        {
            foreach (var u in twice)
            {
                await client.SendTextMessageAsync(u.User.TelegramId, $"Hey, you will be DEAD in a {(u.DateTime - DateTime.Now).ToString()}, so do this task dear");
            }
        }
        private async void Hours()
        {
            foreach (var u in hours_5)
            {
                await client.SendTextMessageAsync(u.User.TelegramId, $"Hey, you will be DEAD in a {(u.DateTime - DateTime.Now).ToString()}, so do this task dear");
            }
        }
        public /*async*/ void Sending()
        {
            if (DateTime.Now.Hour == 12)
            {
                for (int i = 0; i <= senders.Count; i++)
                    senders[i].Invoke();
            }
            else if (DateTime.Now.Hour == 17)
            {
                for (int i = 1; i <= senders.Count; i++)
                    senders[i].Invoke();
            }
            else if (DateTime.Now.Hour == 22)
            {
                for (int i = 2; i <= senders.Count; i++)
                    senders[i].Invoke();
            }
        }
    }
}
