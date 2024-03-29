﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadBot.Enums;

namespace DeadBot.Models
{
    public class DeadLine
    {/* а что если добавить сиайди сюда тогда юзеры не будут нужны..*/
        public long Id { get; set; } /*типо первчиный ключ для бд*/
        public string Name { get; set; }
        public DateTime DateTime { get; set; } /* soft deadline */
        public int Priority { get; set; } /* from 1 to 3, can be used for default frequency */
        public string NotificationFrequency { get; set; } /* может ты знаешь как нормально использовать enum тут? */
        public DateTime StartDate { get; set; }
        public long ChatId {get;set;}
    }
}
