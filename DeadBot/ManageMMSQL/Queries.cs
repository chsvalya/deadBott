using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.ManageMMSQL
{
    class Queries
    {
        public static bool IsNewUser(long cid)
        {
            bool b = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                if (db.DeadLines.FirstOrDefault(x => x.ChatId == cid) == null)
                    b = true;
            }
            return b;
        }
        /*надо наормальный ассинхронный метод для вывода чуть позже я подумаю */
        public static List<DeadLine> Show(long cid)
        {
            var t = new List<DeadLine>();
            using (ApplicationContext db = new ApplicationContext())
            {
                t = db.DeadLines.ToList();
            }
            return t;
        }

        //public static async Task<bool> IsNewUserAsync(long cid) => await Task.Run(() => IsNewUser(cid));

        public static async Task AddDeadline(DeadLine deadLine)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.DeadLines.Add(deadLine);
                await db.SaveChangesAsync();
            }
        }

        public static async Task DeleteDeadLine(DeadLine deadLine)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.DeadLines.Remove(deadLine);
				await db.SaveChangesAsync();
            }
        }

       
        //static async void UpdateDeadLine(long cid,string name)
        //{
        //    using (ApplicationContext db = new ApplicationContext())
        //    {
        //        var d = db.DeadLines.FromSqlRaw()
        //    }
        //}
    }
}
