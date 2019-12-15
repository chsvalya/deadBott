using DeadBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.ManageMMSQL
{
    class Queries
    {
        static void AddNewUser(long cid)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (db.DeadLines.FirstOrDefault(x => x.ChatId == cid) == null)
                    Console.WriteLine();
                    /* хз че дальше делать */
            }
        }
        static async void AddDeadline(DeadLine deadLine)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                await db.DeadLines.AddAsync(deadLine);
                await db.SaveChangesAsync();
            }
        }
        static async void DeleteDeadLine(DeadLine deadLine)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.DeadLines.Remove(deadLine);
                await db.SaveChangesAsync();
            }
        }
        /*вдруг добавим такой функционал,но тогда что меняется ?*/
        //static async void UpdateDeadLine(long cid,string name)
        //{
        //    using (ApplicationContext db = new ApplicationContext())
        //    {
        //        var d = db.DeadLines.FromSqlRaw()
        //    }
        //}
    }
}
