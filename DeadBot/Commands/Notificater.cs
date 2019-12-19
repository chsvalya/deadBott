using DeadBot.ManageMMSQL;
using DeadBot.Models;
using DeadBot.UsefulMethods;
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
        int counter = 0;
        static ITelegramBotClient client;
        static List<Action> senders;
        static List<DeadLine> once;
        static List<DeadLine> twice;
        static List<DeadLine> hours_5;

        public Notificater(ITelegramBotClient c)
        {
            client = c;
            FillIn();
        }

        public void FillIn()
        {
            using (var contxt = new ApplicationContext())
            {
                once = contxt.DeadLines.Where(x => x.NotificationFrequency == "Once a day").ToList();
                twice = contxt.DeadLines.Where(x => x.NotificationFrequency == "Twice a day").ToList();
                hours_5 = contxt.DeadLines.Where(x => x.NotificationFrequency == "Every 5 hours").ToList();
            }

            senders = new List<Action>
            {
                OnceSender,
                TwiceSender,
                Hours
            };
        }

        private void RemindReceivers(IEnumerable<DeadLine> collection)
        {
            foreach (var deadline in collection)
            {

                client.SendTextMessageAsync(deadline.ChatId, $"Hey, you will be DEAD in a " +
                                        $"{(deadline.DateTime - DateTime.Now).Value.Days} days and {(deadline.DateTime - DateTime.Now).Value.Hours} hours, so do this task: " +
                                        $"{deadline.Name}, dear");
            }
        }
        private void OnceSender()
        {
            RemindReceivers(once);
        }
        private void TwiceSender()
        {
            RemindReceivers(twice);
        }
        private void Hours()
        {
            RemindReceivers(hours_5);
        }
        public void Sending()
        {
            if (DateTime.Now.Hour == 17 && DateTime.Now.Second == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Millisecond == 0)
            {
                if (counter == 0)
                {
                    for (int i = 1; i < senders.Count; i++)
                        senders[i].Invoke();
                }
                else
                    counter = 0;
            }
            else if (DateTime.Now.Hour == 22 && DateTime.Now.Second == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Millisecond == 0)
            {
                if (counter == 0)
                {
                    for (int i = 2; i < senders.Count; i++)
                        senders[i].Invoke();
                }
                else
                    counter = 0;
            }
            else if(DateTime.Now.Hour == 12 && DateTime.Now.Second == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Millisecond == 0)
            {
                if (counter == 0)
                {
                    for (int i = 0; i < senders.Count; i++)
                        senders[i].Invoke();
                }
                else
                    counter = 0;
            }
        }
    }
}
