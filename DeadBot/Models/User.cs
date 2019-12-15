using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.Models
{
	public class User
	{
		public int Id { get; set; }
		public long TelegramId { get; set; }
		public bool IsBot { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Username { get; set; }
		public virtual List<DeadLine> DeadLines { get; set; } = new List<DeadLine>();
	}
}
