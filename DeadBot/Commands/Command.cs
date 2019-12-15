﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeadBot.Commands
{
    abstract class Command
    {
        public abstract string Name { get; }

        public abstract void Execute(Message message, TelegramBotClient client);

        public bool Contains(string command)
        {
            return command.Contains(this.Name) &&
                command.Contains(BotSettings.Name);
        }
    }
}
