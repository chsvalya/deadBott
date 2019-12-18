using DeadBot.ManageMMSQL;
using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.UsefulMethods
{
    static class ContextManager
    {
        public static async void GetUser(List<DeadLine> userDeadlines, long chatId, 
                                         Telegram.Bot.Types.User tgUser, int userId)
        {
            using (var context = new ApplicationContext())
            {
                userDeadlines = context.DeadLines.Where(d => d.ChatId == chatId).ToList();
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

        public static async void AddToBd(Dictionary<long, DeadLine> unfinishedDeadlines, long chatId)
        {
            using (var context = new ApplicationContext())
            {
                context.DeadLines.Add(unfinishedDeadlines[chatId]);
                await context.SaveChangesAsync();
            }
        }
    }
}
