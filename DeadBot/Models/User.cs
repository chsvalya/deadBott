using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.Models
{
    class User
    {
        public long ChatId { get; set; }
        public List<DeadLine> DeadLines { get; set; }
        public User(long chatId)
        {
            ChatId = chatId;
            DeadLines = new List<DeadLine>();
        }
        public void AddDeadLine(DeadLine deadLine) => DeadLines.Add(deadLine);
    }
}
