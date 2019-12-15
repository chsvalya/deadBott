using DeadBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.ManageMMSQL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<DeadLine> DeadLines { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=deadd_bot;Trusted_Connection=True;");
        }
    }
}
