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
        public string NotificationFrequency { get; set; } 
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
