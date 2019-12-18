using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadBot.Enums;

namespace DeadBot.Models
{
    public class DeadLine
    {
		public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateTime { get; set; } /* soft deadline */
        public string NotificationFrequency { get; set; } /* может ты знаешь как нормально использовать enum тут? */
        /*крч у меня не получилось старт поменять на строку,из-за обращения к бд,дропнуть таблицы тоже не получается,поэтому пока храним так, а завтра я спрошу
         * у кого-нибудь или ты */
        public DateTime? StartDate { get; set; }
        public long ChatId { get;set; }
		public virtual User User { get; set; }

        public DeadLine()
        {
            Name = null;
            NotificationFrequency = null;
        }
    }
}
