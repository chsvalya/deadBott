using DeadBot.ManageMMSQL;
using DeadBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using static DeadBot.Keyboards.Keyboards;

namespace DeadBot.UsefulMethods
{
    static class AnswerManager
    {
        public static bool IsOldUserWithAllDeadlinesFinished(Dictionary<long, DeadLine> unfinishedDeadlines,
            List<long> wantedDelete, long chatId) => unfinishedDeadlines.Keys.Count(x => x == chatId) == 0 
            && !wantedDelete.Contains(chatId);

        public static bool IsFullDeadline(DeadLine unfinished) =>
            unfinished.Name != null && unfinished.NotificationFrequency != null &&
            unfinished.StartDate != null && unfinished.DateTime != null;

        static string WriteDeadlines(IEnumerable<DeadLine> deadlines)
        {
            var answer = "";
            var counter = 1;
            foreach (var deadline in deadlines)
            {
                answer += $"{counter}: {deadline.Name} on {deadline.DateTime}\n";
                counter++;
            }
            return answer;
        }

        public static async void AddReact(ITelegramBotClient client, 
                                          Dictionary<long, DeadLine> unfinishedDeadlines, long chatId)
        {
            if (!unfinishedDeadlines.ContainsKey(chatId))
            {
                unfinishedDeadlines.Add(chatId, new DeadLine { ChatId = chatId });
            }
            await client.SendTextMessageAsync(chatId, "Enter name of deadline.").ConfigureAwait(false);
        }

        public static async void ShowAllReact(ITelegramBotClient client, List<DeadLine> deadlines, long chatId)
        {
            await client.SendTextMessageAsync(chatId, $"These are all your deadlines:\n{WriteDeadlines(deadlines)}" +
                        $"What's next?", ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);
        }

        public static async void DeleteReact(ITelegramBotClient client, List<long> wantedDelete, 
                                             List<DeadLine> deadlines, long chatId)
        {
            if (!wantedDelete.Contains(chatId))
            {
                wantedDelete.Add(chatId);
            }
            await client.SendTextMessageAsync(chatId, $"These are all your deadlines:\n" +
                        $"{WriteDeadlines(deadlines)} Choose deadline number to delete").ConfigureAwait(false);
        }

        public static async void ClassicGreetings(ITelegramBotClient client, long chatId)
        {
            await client.SendTextMessageAsync(chatId, "What are we doing with deadlines?",
                                      ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);
        }

        public static async void WrongInputReact(ITelegramBotClient client, long chatId)
        {
            await client.SendTextMessageAsync(chatId, "There was a mistake in your input!").ConfigureAwait(false);
        }

        public static int SelectedDeadline(ITelegramBotClient client, long chatId, string text)
        {
            var userResponce = 0;
            if (userResponce == 0 && !int.TryParse(text, out userResponce))
            {
                WrongInputReact(client, chatId);
                return 0;
            }
            return userResponce;
        }

        public static async void DeleteDeadline(ITelegramBotClient client, long chatId, 
                                                DeadLine deleted, List<long> wantedDelete)
        {
            
            await client.SendTextMessageAsync(chatId, $"Deadline \'{deleted.Name}\' deleted!\nWhat's next?",
                                ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);
            wantedDelete.Remove(chatId);
        }

        public static async void ConfigureName(ITelegramBotClient client, DeadLine unfinished, 
                                               long chatId, string text)
        {
            if (!string.IsNullOrEmpty(text))
                unfinished.Name = text;
            Console.WriteLine($"added name: {text}");
            await client.SendTextMessageAsync(chatId, "Choose frequency of reminding", 
                         ParseMode.Default, false, false, 0, frequencyKeyboard).ConfigureAwait(false);
        }

        public static async void ConfigureFrequency(ITelegramBotClient client, DeadLine unfinished, 
                                                    long chatId, string text)
        {
            if (!string.IsNullOrEmpty(text) && (text == "Twice a day" || text == "Once a day" || text == "Every 5 hours"))
            {
                unfinished.NotificationFrequency = text;
                Console.WriteLine($"added freq: {text}");
                await client.SendTextMessageAsync(chatId, "When is the deadline? In the following format" +
                                                                  " YYYY-MM-DD HH:MM:SS").ConfigureAwait(false);
            }
            else
                WrongInputReact(client, chatId);
        }

        public static async void ConfigureDateTime(ITelegramBotClient client, DeadLine unfinished,
                                                   long chatId, string text)
        {
            if (DateTime.TryParse(text, out DateTime Date) && Date > DateTime.Now)
            {
                unfinished.DateTime = Date;
                Console.WriteLine($"added dt: {text}");
                await client.SendTextMessageAsync(chatId, "Wnen should I start to remind? In the following format" +
                                                                  " YYYY-MM-DD HH:MM:SS").ConfigureAwait(false);
            }
            else
                WrongInputReact(client, chatId);
        }

        public static void ConfigureStartDate(ITelegramBotClient client, DeadLine unfinished, long chatId, string text)
        {
            if (DateTime.TryParse(text, out DateTime date))
            {
                unfinished.StartDate = date;
                Console.WriteLine($"added start: {text}");
            }
            else
                WrongInputReact(client, chatId);
        }

        public static async void AddDeadline(ITelegramBotClient client, 
                                             Dictionary<long, DeadLine> unfinishedDeadlines, long chatId)
        {
            await client.SendTextMessageAsync(chatId, "Deadline is added, thanks and good luck!\nWhat's next?",
                        ParseMode.Default, false, false, 0, mainKeyboard).ConfigureAwait(false);

            unfinishedDeadlines.Remove(chatId);
        }
    }
}
