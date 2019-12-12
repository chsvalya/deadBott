using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadBot.Models
{
    /* столкнулась с тем, что надо обращаться к полям бота,
     * поэтому решила сразу иницилизировать их в этом классе
     * по мере необходимости надо будет изменять этот класс*/
    static class BotSettings
    {
        public static string Name { get; set; } = "DeadBot";
        public static string Key { get; set; } = "892374552:AAFcqMqxWkCsHZNQYj2G_mhgZ6ERDek4m6Q";
    }
}
