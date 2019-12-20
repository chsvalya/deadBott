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

        private void FillIn()
        {
            using (var contxt = new ApplicationContext())
            {
                once = contxt.DeadLines.Where(d =>  FindDeadlines(d, "Once a day")).ToList();
                twice = contxt.DeadLines.Where(d => FindDeadlines(d, "Twice a day")).ToList();
                hours_5 = contxt.DeadLines.Where(d => FindDeadlines(d, "Every 5 hours")).ToList();
            }

            senders = new List<Action>
            {
                OnceSender,
                TwiceSender,
                Hours
            };
        }

        private bool FindDeadlines(DeadLine deadLine, string frequency) =>
            (deadLine.NotificationFrequency == frequency && deadLine.StartDate <= DateTime.Now
                                                         && deadLine.DateTime >= DateTime.Now);

        private bool CheckTimeToNotificate(int hours) =>
            (DateTime.Now.Hour == hours && DateTime.Now.Second == 0 &&
             DateTime.Now.Minute == 0 && DateTime.Now.Millisecond == 0);

        private void SendNotifications(int start)
        {
            if (counter == 0)
            {
                for (int i = start; i < senders.Count; i++)
                    senders[i].Invoke();
            }
            else
                counter = 0;
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
            if (CheckTimeToNotificate(12))
            {
                SendNotifications(0);
            }
            else if (CheckTimeToNotificate(17))
            {
                SendNotifications(1);
            }
            else if (CheckTimeToNotificate(22))
            {
                SendNotifications(2);
            }
        }
    }
}
