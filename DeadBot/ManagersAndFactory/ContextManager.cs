using DeadBot.ManageMMSQL;
using DeadBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace DeadBot.ManagersAndFactory
{
    static class ContextManager
    {
        static readonly Dictionary<long, DeadLine> UsersAndUnfinishedDeadlines = Factory.Instance.GetUsersAndUnfinishedDeadlines;
        public static async void GetUser(Telegram.Bot.Types.User tgUser, int userId)
        {
            using (var context = new ApplicationContext())
            {
                var dbuser = context.Users
                    .FirstOrDefault(value => value.TelegramId == tgUser.Id);

                if (dbuser == default)
                {
                    dbuser = new User
                    {
                        FirstName = tgUser.FirstName,
                        IsBot = tgUser.IsBot,
                        LastName = tgUser.LastName,
                        Username = tgUser.Username,
                        TelegramId = tgUser.Id
                    };

                    context.Users.Add(dbuser);
                    await context.SaveChangesAsync();
                }

                userId = dbuser.Id;
            }
        }

        public static async void DeleteFromBd(DeadLine deleted)
        {
            using (var context = new ApplicationContext())
            {
                context.DeadLines.Attach(deleted);
                context.DeadLines.Remove(deleted);
                await context.SaveChangesAsync();
            }
        }

        public static async void AddToBd(long chatId)
        {
            using (var context = new ApplicationContext())
            {
                context.DeadLines.Add(UsersAndUnfinishedDeadlines[chatId]);
                await context.SaveChangesAsync();
            }
        }
    }
}
