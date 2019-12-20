using DeadBot.ManageMMSQL;
using DeadBot.Models;
using DeadBot.ManagersAndFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;

namespace DeadBot.Notifications
{
    class Notificater
    {
        int counter = 0;
        static ITelegramBotClient client = Factory.Instance.GetClient;
        static List<Action> senders;
        static List<DeadLine> once;
        static List<DeadLine> twice;
        static List<DeadLine> hours_5;

        public Notificater()
        {
            FillIn();
        }

        public void FillIn()
        {
            using (var contxt = new ApplicationContext())
            {
                once = contxt.DeadLines.Where(x => x.NotificationFrequency == "Once a day" && x.StartDate <= DateTime.Now && x.DateTime >= DateTime.Now).ToList();
                twice = contxt.DeadLines.Where(x => x.NotificationFrequency == "Twice a day" && x.StartDate <= DateTime.Now && x.DateTime >= DateTime.Now).ToList();
                hours_5 = contxt.DeadLines.Where(x => x.NotificationFrequency == "Every 5 hours" && x.StartDate <= DateTime.Now && x.DateTime >= DateTime.Now).ToList();
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
                                        $"{(deadline.DateTime - DateTime.Now).Value.Days} days,{(deadline.DateTime - DateTime.Now).Value.Hours} hours, " +
                                        $"{(deadline.DateTime - DateTime.Now).Value.Minutes} minutes and {(deadline.DateTime - DateTime.Now).Value.Seconds} seconds, so do this task: " +
                                        $"{deadline.Name}, dear");
            }
            foreach (var deadline in collection)
                AnswerManager.ShowPicture(deadline.ChatId);
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
