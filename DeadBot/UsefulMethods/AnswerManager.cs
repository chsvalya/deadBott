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
        public static Random rnd = new Random();
        public static List<String> pictures = new List<string>
        {
            "https://www.google.com/imgres?imgurl=https://gif.cmtt.space/3/club/7d/a2/af/d857d4ba235e34.jpg&imgrefurl=https://tjournal.ru/flood/32290-mem-this-is-fine-poluchil-prodolzhenie&tbnid=TM8Yc0_veQ5jXM&vet=1&docid=k0xJVGrjee3IFM&w=895&h=910&q=%D0%BC%D0%B5%D0%BC+this+is+fine&hl=ru-RU&source=sh/x/im",
            "https://www.google.com/imgres?imgurl=https://flamingo-petr.ru/images/1557006-image.jpg&imgrefurl=https://flamingo-petr.ru/product/-krugka-goryashii-dedlain-&tbnid=I1wBtmYkS48OQM&vet=1&docid=h6Px-av6iopWKM&w=400&h=400&q=%D0%B3%D0%BE%D1%80%D1%8F%D1%89%D0%B8%D0%B9+%D0%B4%D0%B5%D0%B4%D0%BB%D0%B0%D0%B9%D0%BD&hl=ru-RU&source=sh/x/im",
            "https://www.google.com/imgres?imgurl=http://img1.reactor.cc/pics/post/Cat%2527s-cafe-%25D0%25BA%25D0%25B0%25D1%2584%25D0%25B5-%25D0%25BA%25D0%25BE%25D1%2582%25D0%25B8%25D0%25BA%25D0%25B0-%25D0%259A%25D0%25BE%25D0%25BC%25D0%25B8%25D0%25BA%25D1%2581%25D1%258B-%25D0%25BC%25D0%25B8%25D0%25BB%25D0%25BE%25D1%2582%25D0%25B0-4672525.jpeg&imgrefurl=http://reactor.cc/post/3624137&tbnid=y5mdIBZD24-adM&vet=1&docid=MdXMZJGyYrwPvM&w=800&h=1099&itg=1&q=%D0%B3%D0%BE%D1%80%D1%8F%D1%89%D0%B8%D0%B9+%D0%B4%D0%B5%D0%B4%D0%BB%D0%B0%D0%B9%D0%BD&hl=ru-RU&source=sh/x/im",
            "https://www.google.com/imgres?imgurl=https://cdn.lifehacker.ru/wp-content/uploads/2015/08/cover-01_1438513639.png&imgrefurl=https://lifehacker.ru/deadline/&tbnid=U2raavf7y0K0uM&vet=1&docid=APZEwPPf_udIDM&w=2667&h=1333&q=%D0%B3%D0%BE%D1%80%D1%8F%D1%89%D0%B8%D0%B9+%D0%B4%D0%B5%D0%B4%D0%BB%D0%B0%D0%B9%D0%BD&hl=ru-RU&source=sh/x/im",
            "https://www.google.com/imgres?imgurl=http://e-kazan.ru/upload/gallery/photo_2388.jpg&imgrefurl=http://e-kazan.ru/news/show/42097&tbnid=-4UD2l3u9wdTtM&vet=1&docid=3nowjvHp7ZzzgM&w=503&h=335&q=%D0%B3%D0%BE%D1%80%D1%8F%D1%89%D0%B8%D0%B9+%D0%B4%D0%B5%D0%B4%D0%BB%D0%B0%D0%B9%D0%BD&hl=ru-RU&source=sh/x/im"

        };
        
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
            ShowPicture(client, chatId);
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
        public static async void ShowPicture(ITelegramBotClient client,long chatId)
        {
            await client.SendPhotoAsync(chatId, pictures[rnd.Next(0, pictures.Count)]);
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
