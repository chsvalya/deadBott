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
        static readonly ITelegramBotClient client = Factory.Instance.GetClient;
        static List<Action> senders;
        static List<DeadLine> once;
        static List<DeadLine> twice;
        static List<DeadLine> hours_5;
        readonly Func<DeadLine, bool> findOnce;
        readonly Func<DeadLine, bool> findTwice;
        readonly Func<DeadLine, bool> findHours5;

        public Notificater()
        {
            findOnce = FindOnceDeadlines;
            findTwice = FindTwiceDeadlines;
            findHours5 = FindHours5Deadlines;
            FillIn();
        }

        private void FillIn()
        {
            using (var contxt = new ApplicationContext())
            {
                once = contxt.DeadLines.Where(findOnce).ToList();
                twice = contxt.DeadLines.Where(findTwice).ToList();
                hours_5 = contxt.DeadLines.Where(findHours5).ToList();
            }

            senders = new List<Action>
            {
                OnceSender,
                TwiceSender,
                Hours
            };
        }

        private bool CheckDate(DeadLine deadLine) => deadLine.StartDate <= DateTime.Now
                                                  && deadLine.DateTime >= DateTime.Now;
        private bool FindOnceDeadlines(DeadLine deadLine) =>
            (deadLine.NotificationFrequency == "Once a day" && CheckDate(deadLine));

        private bool FindTwiceDeadlines(DeadLine deadLine) =>
            (deadLine.NotificationFrequency == "Twice a day" && CheckDate(deadLine));

        private bool FindHours5Deadlines(DeadLine deadLine) =>
            (deadLine.NotificationFrequency == "Every 5 hours" && CheckDate(deadLine));

        private bool CheckTimeToNotificate(int hours) =>
            (DateTime.Now.Hour == hours && DateTime.Now.Second == 0 &&
             DateTime.Now.Minute == 15 && DateTime.Now.Millisecond == 0);

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
            foreach (var deadline in collection.GroupBy(x => x.ChatId).Select(x => x.First()).ToList())
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
            else if (CheckTimeToNotificate(23))
            {
                SendNotifications(2);
            }
        }
    }
}
