namespace LotteryTicketChecker.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Timers;

    using AngleSharp.Parser.Html;

    public class Checker
    {
        private readonly HtmlParser parser = new HtmlParser();
        private CookieContainer cookieContainer = new CookieContainer();
        private readonly Timer timer;
        private string passwordCache;
        private string usernameCache;

        public Checker()
        {
            this.timer = new Timer();
            this.timer.Interval = 360000;
            this.timer.Elapsed += this.OnTimer;
        }

        public bool IsLoggedIn { get; set; }

        public void Start()
        {
            this.timer.Start();
        }

        public async Task<bool> Login(string username, string password)
        {
            this.usernameCache = username;
            this.passwordCache = password;

            this.cookieContainer = new CookieContainer();

            var handler = new HttpClientHandler();
            handler.CookieContainer = this.cookieContainer;

            using (var client = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(new[] {
                                                                        new KeyValuePair<string, string>("login_username", username),
                                                                        new KeyValuePair<string, string>("login_password", password),
                                                                        new KeyValuePair<string, string>("submit_form", "1")
                                                              });

                var response = await client.PostAsync("https://7777.bg/login/", content);
                var result = await response.Content.ReadAsStringAsync();
                var document = this.parser.Parse(result);
                var title = document.Title;
                if (title.ToLower().Contains("вход"))
                {
                    this.IsLoggedIn = false;
                    return false;
                }

                this.IsLoggedIn = true;
                return true;
            }
        }

        public async Task<string> Check(string barcode)
        {
            if (barcode.Length != 14 && barcode.Length != 13)
            {
                return "Невалиден баркод";
            }

            var  handler = new HttpClientHandler();
            handler.CookieContainer = this.cookieContainer;
            using (var client = new HttpClient(handler))
            {
                var response = await client.GetAsync($"https://7777.bg/loto_games/check_talon/?talon_id={barcode}");
                var result = await response.Content.ReadAsStringAsync();
                var document = this.parser.Parse(result);
                var winning = document.GetElementsByClassName("winnig-text").FirstOrDefault();
                if (winning != null)
                {
                    return winning.TextContent;
                }

                var status = document.GetElementsByClassName("bordered").FirstOrDefault()?.GetElementsByTagName("h1").FirstOrDefault();
                if (status != null)
                {
                    return status.TextContent;
                }
                
                return "Грешка";
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            this.Login(this.usernameCache, this.passwordCache);
        }
    }
}