using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace DeadBot.Keyboards
{
    static class Keyboards
    {
        public static ReplyKeyboardMarkup mainKeyboard = new ReplyKeyboardMarkup(
            new KeyboardButton[][]
            {
                new []
                {
                    new KeyboardButton("Add"),
                    new KeyboardButton("Delete"),
                    new KeyboardButton("Show all")
                }
            }, oneTimeKeyboard: true);

        public static ReplyKeyboardMarkup frequencyKeyboard = new ReplyKeyboardMarkup(
            new KeyboardButton[][]
            {
                new []
                {
                    new KeyboardButton("Two weeks"),
                    new KeyboardButton("One week")
                },
                new[]
                {
                    new KeyboardButton("Five days"),
                    new KeyboardButton("Default")
                }
            }, oneTimeKeyboard: true);

        public static ReplyKeyboardMarkup startDateKeyboard = new ReplyKeyboardMarkup(
            new KeyboardButton[][]
            {
                new []
                {
                    new KeyboardButton("Once a day"),
                    new KeyboardButton("Twice a day")
                },
                new[]
                {
                    new KeyboardButton("Every 5 hours"),
                    new KeyboardButton("Default")
                }
            }, oneTimeKeyboard: true);
    }
}
