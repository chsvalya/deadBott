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
                    new KeyboardButton("Once a day"),
                    new KeyboardButton("Twice a day"),
                    new KeyboardButton("Every 5 hours")
                }
            }, oneTimeKeyboard: true);
    }
}
