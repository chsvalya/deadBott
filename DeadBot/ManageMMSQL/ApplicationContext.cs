using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.ManageMMSQL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<DeadLine> DeadLines { get; set; }
		public DbSet<User> Users { get; set; }

        public ApplicationContext() : base("Server=(localdb)\\mssqllocaldb;Database=deadd_bot;Trusted_Connection=True;")
        {
        }
	}
}
