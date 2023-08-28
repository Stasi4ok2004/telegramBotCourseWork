using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBotKursach
{

    public class ResponseTopScorers
    {

    }


    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) { return value; }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }

    class Program
    {
        private static bool isSent = false;

        static string baseUrl = "https://api-football-v1.p.rapidapi.com/v3/";

        static string[] categories = new string[]
            {
                "Top scorers of EPL from 2010",
                "Top assists of EPL from 2014",
                "Top Red Cards of EPL from 2010",
                "Top Yellow Cards of EPL from 2010",
                "Current standings",
                "Today`s matches"
            };
        static string[] leagues = new string[]
            {
                "English Premier League",
                "Serie A",
                "La Liga",
                "Bundesliga",
                "Ligue 1",
                "Ukrainian Premier League"
            };
        static string responseMessage = "response";
        static HttpClient httpClient = new HttpClient();





        private static string token { get; set; } = "6382254092:AAETnCMO9YSZzzUitua25dh8TLBzVma3pko";
        private static TelegramBotClient client = new TelegramBotClient(token);

        static void Main(string[] args)
        {

            client.StartReceiving(UpdateHandler, ErrorHandler);


            Console.ReadKey();
        }

        async static Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            //SearchTopscorer(botClient, "39", update, "2020");

            if (message.Text != null)
            {
                //botClient.SendTextMessageAsync(message.Chat.Id, "Choose one of the following", replyMarkup: keyboardBuilder(categories));
                OneTimeMessage(botClient, update);

                Console.WriteLine($"{message.Chat.Id} || {message.Date} || {message.Text}");

                //await CurrentStandings(botClient, "39", update);

                if (message.Text == "Top scorers of EPL from 2010")
                {
                    //InlineKeyboardMarkup keyboard = InlineKeyboardBuilder(leagues);
                    //SearchTopscorer(botClient, "39", update, "2020");

                    // await botClient.SendTextMessageAsync(message.Chat.Id, "Which league`s top goalscorer you want to know?", replyMarkup: ReplyKeyboardBuilder(leagues));
                    for (int i = 2010; i < 2023; i++)
                    {
                        await SearchTopscorer(botClient, "39", update, i.ToString());

                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Choose another: ", replyMarkup: ReplyKeyboardBuilder(categories));
                }

                else if (message.Text == "Top assists of EPL from 2014")
                {

                    for (int i = 2014; i < 2023; i++)
                    {
                        await SearchTopAssister(botClient, "39", update, i.ToString());

                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Choose another: ", replyMarkup: ReplyKeyboardBuilder(categories));
                }

                else if (message.Text == "Top Red Cards of EPL from 2010")
                {
                    for (int i = 2010; i < 2023; i++)
                    {
                        await SearchTopRedCards(botClient, "39", update, i.ToString());

                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Choose another: ", replyMarkup: ReplyKeyboardBuilder(categories));
                }

                else if (message.Text == "Top Yellow Cards of EPL from 2010")
                {
                    for (int i = 2010; i < 2023; i++)
                    {
                        await SearchTopYellowCards(botClient, "39", update, i.ToString());
                    }
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Choose another: ", replyMarkup: ReplyKeyboardBuilder(categories));
                }
                else if (message.Text == "Current standings")
                {
                    await CurrentStandings(botClient, "39", update);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Choose another: ", replyMarkup: ReplyKeyboardBuilder(categories));

                }
                else if (message.Text == "Today`s matches")
                {
                    await TodaysMatches(botClient, "39", update);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Choose another: ", replyMarkup: ReplyKeyboardBuilder(categories));

                }
            }
        }


        private static IReplyMarkup ReplyKeyboardBuilder(string[] buttonTexts)
        {


            KeyboardButton[][] keyboardButtons = buttonTexts.Select(buttonText => new KeyboardButton[] { buttonText }).ToArray();

            ReplyKeyboardMarkup rkm = new ReplyKeyboardMarkup(keyboardButtons) { ResizeKeyboard = true, OneTimeKeyboard = true };

            return rkm;
        }


        private static InlineKeyboardMarkup InlineKeyboardBuilder(string[] buttonTexts)
        {
            InlineKeyboardButton[][] inlineKeyboardButtons = buttonTexts.Select(buttonText => new InlineKeyboardButton[] { buttonText }).ToArray();

            InlineKeyboardMarkup rkm = new InlineKeyboardMarkup(inlineKeyboardButtons);

            return rkm;
        }

        private async static void OneTimeMessage(ITelegramBotClient botClient, Update update)
        {
            if (update.Message.Type == MessageType.Text && !isSent)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Choose one of the following", replyMarkup: ReplyKeyboardBuilder(categories));
                isSent = true;
            }
        }

        async static Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync("wtf everything is broken again");
        }


        public static async Task SearchTopscorer(ITelegramBotClient botClient, string leagueId, Update update, string season)
        {


            string longurl = "https://api-football-v1.p.rapidapi.com/v3/players/topscorers?";
            var uriBuilder = new UriBuilder(longurl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["league"] = leagueId;
            query["season"] = season;
            uriBuilder.Query = query.ToString();
            longurl = uriBuilder.ToString();
            //Console.WriteLine(longurl);



            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(longurl),
                Headers =
                {
                    { "X-RapidAPI-Key", "cc7ace5245msh73985525a6ac7b8p1a57f5jsn13b8b2f85a35" },
                    { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(body.Truncate(20));

                dynamic jsonData = JObject.Parse(body);
                // Console.WriteLine(jsonData.ToString());

                string name = jsonData.response[0].player.name;

                string goals = jsonData.response[0].statistics[0].goals.total;

                int games = jsonData.response[0].statistics[0].games.appearences;
                string league = jsonData.response[0].statistics[0].league.name;
                //Console.WriteLine($"{name} is topscorer in {season} with {goals} goals");


                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{name} is topscorer in {season} with {goals} goals in {games} appearences");

                /*Console.WriteLine(responseMessage);
                Console.WriteLine(baseUrl);
                */
            }
        }
        public static async Task SearchTopAssister(ITelegramBotClient botClient, string leagueId, Update update, string season)
        {


            string longurl = "https://api-football-v1.p.rapidapi.com/v3/players/topassists?";
            var uriBuilder = new UriBuilder(longurl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["league"] = leagueId;
            query["season"] = season;
            uriBuilder.Query = query.ToString();
            longurl = uriBuilder.ToString();




            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(longurl),
                Headers =
                {
                    { "X-RapidAPI-Key", "cc7ace5245msh73985525a6ac7b8p1a57f5jsn13b8b2f85a35" },
                    { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(body.Truncate(20));

                dynamic jsonData = JObject.Parse(body);


                string name = jsonData.response[0].player.name;
                //Console.WriteLine(jsonData.response[0].player.name);

                int assists = jsonData.response[0].statistics[0].goals.assists;

                int games = jsonData.response[0].statistics[0].games.appearences;
                string league = jsonData.response[0].statistics[0].league.name;



                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{name} is topscorer in {season} in {league} with {assists} assists in {games} appearences");

                /*Console.WriteLine(responseMessage);
                Console.WriteLine(baseUrl);
                */
            }
        }

        public static async Task SearchTopRedCards(ITelegramBotClient botClient, string leagueId, Update update, string season)
        {


            string longurl = "https://api-football-v1.p.rapidapi.com/v3/players/topredcards?";
            var uriBuilder = new UriBuilder(longurl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["league"] = leagueId;
            query["season"] = season;
            uriBuilder.Query = query.ToString();
            longurl = uriBuilder.ToString();
            //Console.WriteLine(longurl);



            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(longurl),
                Headers =
                {
                    { "X-RapidAPI-Key", "cc7ace5245msh73985525a6ac7b8p1a57f5jsn13b8b2f85a35" },
                    { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(body.Truncate(20));

                dynamic jsonData = JObject.Parse(body);
                // Console.WriteLine(jsonData.ToString());

                string name = jsonData.response[0].player.name;

                int reds = jsonData.response[0].statistics[0].cards.red + jsonData.response[0].statistics[0].cards.yellowred;

                int games = jsonData.response[0].statistics[0].games.appearences;
                string league = jsonData.response[0].statistics[0].league.name;
                //Console.WriteLine($"{name} is topscorer in {season} with {goals} goals");


                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{name} has the most red cards in {season} with {reds} reds in {games} games");

                /*Console.WriteLine(responseMessage);
                Console.WriteLine(baseUrl);
                */
            }
        }

        public static async Task SearchTopYellowCards(ITelegramBotClient botClient, string leagueId, Update update, string season)
        {


            string longurl = "https://api-football-v1.p.rapidapi.com/v3/players/topyellowcards?";
            var uriBuilder = new UriBuilder(longurl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["league"] = leagueId;
            query["season"] = season;
            uriBuilder.Query = query.ToString();
            longurl = uriBuilder.ToString();
            //Console.WriteLine(longurl);



            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(longurl),
                Headers =
                {
                    { "X-RapidAPI-Key", "cc7ace5245msh73985525a6ac7b8p1a57f5jsn13b8b2f85a35" },
                    { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(body.Truncate(20));

                dynamic jsonData = JObject.Parse(body);
                // Console.WriteLine(jsonData.ToString());

                string name = jsonData.response[0].player.name;

                int yellows = jsonData.response[0].statistics[0].cards.yellow;

                int games = jsonData.response[0].statistics[0].games.appearences;
                string league = jsonData.response[0].statistics[0].league.name;
                //Console.WriteLine($"{name} is topscorer in {season} in {league} with {goals} goals");


                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"{name} has the most yellow cards in {season} with {yellows} yellows in {games} games");

                /*Console.WriteLine(responseMessage);
                Console.WriteLine(baseUrl);
                */
            }
        }

        public static async Task CurrentStandings(ITelegramBotClient botClient, string leagueId, Update update)
        {


            string longurl = "https://api-football-v1.p.rapidapi.com/v3/standings?";
            var uriBuilder = new UriBuilder(longurl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["league"] = leagueId;
            query["season"] = "2023";
            uriBuilder.Query = query.ToString();
            longurl = uriBuilder.ToString();
            //Console.WriteLine(longurl);



            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(longurl),
                Headers =
                {
                    { "X-RapidAPI-Key", "cc7ace5245msh73985525a6ac7b8p1a57f5jsn13b8b2f85a35" },
                    { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                string table = "TEAM || POINTS || GOALS DIFF\n";
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(body.Truncate(20));

                dynamic jsonData = JObject.Parse(body);

                for (int i = 0; i < 20; i++)
                {

                    string name = jsonData.response[0].league.standings[0][i].team.name;
                    string points = jsonData.response[0].league.standings[0][i].points;
                    string gD = jsonData.response[0].league.standings[0][i].goalsDiff;

                    table += $"{name} - {points} - {gD}\n";
                }
                //Console.WriteLine(table);

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, table);

                /*Console.WriteLine(responseMessage);
                Console.WriteLine(baseUrl);
                */
            }
        }
        public static async Task TodaysMatches(ITelegramBotClient botClient, string leagueId, Update update)
        {


            string longurl = "https://api-football-v1.p.rapidapi.com/v3/fixtures?";
            var uriBuilder = new UriBuilder(longurl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["date"] = DateTime.Now.ToString("yyyy-MM-dd");
            query["league"] = leagueId;
            query["season"] = DateTime.Now.ToString("yyyy");
            uriBuilder.Query = query.ToString();
            longurl = uriBuilder.ToString();
            //Console.WriteLine(longurl);



            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(longurl),
                Headers =
                {
                    { "X-RapidAPI-Key", "cc7ace5245msh73985525a6ac7b8p1a57f5jsn13b8b2f85a35" },
                    { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                string result = "Today are played next matches\n";
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(body.Truncate(20));

                dynamic jsonData = JObject.Parse(body);

                if (jsonData.response.Count == 0)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "No one`s playing today((((");
                }
                else
                {
                    for (int i = 0; i < jsonData.response.Count; i++)
                    {

                        result += jsonData.response[i].teams.home.name + " vs " + jsonData.response[i].teams.away.name + "\n";

                    }


                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, result);


                }
            }
        }
    }
}
